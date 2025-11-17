using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.AnalyticsServiceAPI.Contracts;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeoCidadao.AnalyticsServiceAPI.BackgroundServices
{
    public class PostAnalyticsConsumerService : BackgroundService
    {
        private readonly ILogger<PostAnalyticsConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private AnalyticsRabbitMQSubscriber? _subscriber;

        public PostAnalyticsConsumerService(
            ILogger<PostAnalyticsConsumerService> logger,
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
                // Create subscriber instance
                using var scope = _scopeFactory.CreateScope();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var subscriberLogger = loggerFactory.CreateLogger<RabbitMQSubscriberService>();
                
                _subscriber = new AnalyticsRabbitMQSubscriber(subscriberLogger, _configuration);

                // Subscribe to analytics queue
                _subscriber.ConsumeQueue(
                    exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                    queueName: QueueNames.ANALYTICS_SERVICE_POST_ANALYTICS_QUEUE_NAME,
                    routingKey: RoutingKeyNames.POST_ANALYTICS_ROUTING_KEY,
                    onNewMessage: OnMessageReceived,
                    dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                    dlqQueueName: QueueNames.DLQ_ANALYTICS_SERVICE_POST_ANALYTICS_QUEUE_NAME,
                    dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_ANALYTICS_ROUTING_KEY,
                    deliveryLimit: 3
                );

                _logger.LogInformation("Analytics consumer service started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting analytics consumer service: {ex.Message}");
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
                
                _logger.LogDebug($"Received analytics message: {message}");

                var analyticsMessage = JsonSerializer.Deserialize<PostAnalyticsMessage>(message);
                
                if (analyticsMessage == null)
                {
                    _logger.LogWarning("Failed to deserialize analytics message");
                    // Acknowledge to remove from queue
                    _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
                    return;
                }

                // Process message
                using var scope = _scopeFactory.CreateScope();
                var processingService = scope.ServiceProvider.GetRequiredService<IAnalyticsProcessingService>();
                
                await processingService.ProcessPostAnalyticsEventAsync(analyticsMessage);

                // Acknowledge successful processing
                _subscriber?.AcknowledgeMessage(eventArgs.DeliveryTag);
                
                _logger.LogInformation($"Successfully processed analytics event for post {analyticsMessage.PostId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing analytics message: {ex.Message}");
                
                // Reject and requeue (will go to DLQ after delivery limit)
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
    internal class AnalyticsRabbitMQSubscriber : RabbitMQSubscriberService
    {
        public AnalyticsRabbitMQSubscriber(ILogger<RabbitMQQueueService> logger, IConfiguration configuration) 
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
}
