using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeoCidadao.SearchServiceAPI.BackgroundServices;

/// <summary>
/// Background service to listen for post change events and update the search index
/// </summary>
public class PostChangedSubscriberService : BackgroundService
{
    private readonly ILogger<PostChangedSubscriberService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private SearchRabbitMQSubscriber? _subscriber;

    public PostChangedSubscriberService(
        ILogger<PostChangedSubscriberService> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var subscriberLogger = loggerFactory.CreateLogger<RabbitMQSubscriberService>();
            
            _subscriber = new SearchRabbitMQSubscriber(subscriberLogger, _configuration);

            _subscriber.ConsumeQueue(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: "search-service-post-changed-queue",
                routingKey: RoutingKeyNames.POST_CHANGED_ROUTING_KEY,
                onNewMessage: OnMessageReceived,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: "search-service-post-changed-queue-dlq",
                dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_CHANGED_ROUTING_KEY,
                deliveryLimit: 3
            );

            _logger.LogInformation("Post changed subscriber service started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting post changed subscriber service");
            throw;
        }

        return Task.CompletedTask;
    }

    private async void OnMessageReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogDebug("Received post changed message: {Message}", message);

            var postChangedMessage = JsonSerializer.Deserialize<PostChangedMessage>(message);
            
            if (postChangedMessage == null)
            {
                _logger.LogWarning("Failed to deserialize post changed message");
                _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var postDataService = scope.ServiceProvider.GetRequiredService<IPostDataService>();
            var indexService = scope.ServiceProvider.GetRequiredService<IElasticsearchIndexService>();

            _logger.LogInformation("Processing post change event for post {PostId}", postChangedMessage.PostId);

            var postDocument = await postDataService.GetPostByIdAsync(postChangedMessage.PostId);
            
            if (postDocument != null)
            {
                await indexService.UpdatePostAsync(postChangedMessage.PostId, postDocument);
                _logger.LogInformation("Post {PostId} indexed successfully", postChangedMessage.PostId);
            }
            else
            {
                // Post was deleted, remove from index
                await indexService.DeletePostAsync(postChangedMessage.PostId);
                _logger.LogInformation("Post {PostId} removed from index", postChangedMessage.PostId);
            }

            _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing post changed message");
            _subscriber?.RejectMessage(eventArgs.DeliveryTag);
        }
    }

    public override void Dispose()
    {
        _subscriber?.Dispose();
        base.Dispose();
    }
}

// Helper class to expose channel methods
internal class SearchRabbitMQSubscriber : RabbitMQSubscriberService
{
    public SearchRabbitMQSubscriber(ILogger<RabbitMQQueueService> logger, IConfiguration configuration) 
        : base(logger, configuration)
    {
    }

    public void AcknowledgeMessage(ulong deliveryTag)
    {
        Channel?.BasicAck(deliveryTag, false);
    }

    public void RejectMessage(ulong deliveryTag)
    {
        Channel?.BasicNack(deliveryTag, false, false);
    }
}
