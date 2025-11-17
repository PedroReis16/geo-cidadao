using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using GeoCidadao.SearchServiceAPI.Models;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeoCidadao.SearchServiceAPI.BackgroundServices;

/// <summary>
/// Background service to listen for new user events and index them
/// </summary>
public class NewUserSubscriberService : BackgroundService
{
    private readonly ILogger<NewUserSubscriberService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private SearchRabbitMQSubscriber? _subscriber;

    public NewUserSubscriberService(
        ILogger<NewUserSubscriberService> logger,
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
                exchangeName: ExchangeNames.KEYCLOAK_EVENTS_TOPIC_EXCHANGE,
                queueName: "search-service-new-user-queue",
                routingKey: RoutingKeyNames.NEW_USER_ROUTING_KEY,
                onNewMessage: OnMessageReceived,
                dlqExchangeName: ExchangeNames.DLQ_KEYCLOAK_EVENTS_TOPIC_EXCHANGE,
                dlqQueueName: "search-service-new-user-queue-dlq",
                dlqRoutingKeyName: RoutingKeyNames.DLQ_NEW_USER_ROUTING_KEY,
                deliveryLimit: 3
            );

            _logger.LogInformation("New user subscriber service started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting new user subscriber service");
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
            
            _logger.LogDebug("Received new user message: {Message}", message);

            var newUserMessage = JsonSerializer.Deserialize<NewUserMessage>(message);
            
            if (newUserMessage == null)
            {
                _logger.LogWarning("Failed to deserialize new user message");
                _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var indexService = scope.ServiceProvider.GetRequiredService<IElasticsearchIndexService>();

            _logger.LogInformation("Processing new user event for user {UserId}", newUserMessage.Id);

            var userDocument = new UserDocument
            {
                Id = Guid.Parse(newUserMessage.Id),
                Username = newUserMessage.Username,
                Email = newUserMessage.Email,
                FirstName = newUserMessage.FirstName,
                LastName = newUserMessage.LastName,
                FullName = $"{newUserMessage.FirstName} {newUserMessage.LastName}".Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsActive = true
            };

            await indexService.IndexUserAsync(userDocument);
            _logger.LogInformation("New user {UserId} indexed successfully", newUserMessage.Id);

            _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing new user message");
            _subscriber?.RejectMessage(eventArgs.DeliveryTag);
        }
    }

    public override void Dispose()
    {
        _subscriber?.Dispose();
        base.Dispose();
    }
}
