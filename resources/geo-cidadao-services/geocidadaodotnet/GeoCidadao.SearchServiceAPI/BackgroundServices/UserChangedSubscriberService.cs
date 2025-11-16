using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeoCidadao.SearchServiceAPI.BackgroundServices;

/// <summary>
/// Background service to listen for user change events and update the search index
/// </summary>
public class UserChangedSubscriberService : BackgroundService
{
    private readonly ILogger<UserChangedSubscriberService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private SearchRabbitMQSubscriber? _subscriber;

    public UserChangedSubscriberService(
        ILogger<UserChangedSubscriberService> logger,
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
                exchangeName: ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: "search-service-user-changed-queue",
                routingKey: RoutingKeyNames.USER_CHANGED_ACTIONS_ROUTING_KEY,
                onNewMessage: OnMessageReceived,
                dlqExchangeName: ExchangeNames.DLQ_USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: "search-service-user-changed-queue-dlq",
                dlqRoutingKeyName: RoutingKeyNames.DLQ_USER_CHANGED_ACTIONS_ROUTING_KEY,
                deliveryLimit: 3
            );

            _logger.LogInformation("User changed subscriber service started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting user changed subscriber service");
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
            
            _logger.LogDebug("Received user changed message: {Message}", message);

            var userChangedMessage = JsonSerializer.Deserialize<UserChangedMessage>(message);
            
            if (userChangedMessage == null)
            {
                _logger.LogWarning("Failed to deserialize user changed message");
                _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var userDataService = scope.ServiceProvider.GetRequiredService<IUserDataService>();
            var indexService = scope.ServiceProvider.GetRequiredService<IElasticsearchIndexService>();

            _logger.LogInformation("Processing user change event for user {UserId}", userChangedMessage.UserId);

            var userDocument = await userDataService.GetUserByIdAsync(userChangedMessage.UserId);
            
            if (userDocument != null)
            {
                await indexService.UpdateUserAsync(userChangedMessage.UserId, userDocument);
                _logger.LogInformation("User {UserId} indexed successfully", userChangedMessage.UserId);
            }
            else
            {
                // User was deleted, remove from index
                await indexService.DeleteUserAsync(userChangedMessage.UserId);
                _logger.LogInformation("User {UserId} removed from index", userChangedMessage.UserId);
            }

            _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user changed message");
            _subscriber?.RejectMessage(eventArgs.DeliveryTag);
        }
    }

    public override void Dispose()
    {
        _subscriber?.Dispose();
        base.Dispose();
    }
}
