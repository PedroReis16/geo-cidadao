using GeoCidadao.AMQP.Messages;
using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.Entities;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using GeoCidadao.AMQP.Configuration;

namespace GeoCidadao.AnalyticsServiceAPI.Services.BackgroundServices
{
    public class PostCreatedConsumerService : BackgroundService
    {
        private readonly ILogger<PostCreatedConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IModel? _channel;

        public PostCreatedConsumerService(
            ILogger<PostCreatedConsumerService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest"
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange
                _channel.ExchangeDeclare(
                    exchange: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    arguments: null
                );

                // Declare queue
                var queueName = "analytics_post_created_queue";
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME },
                        { "x-dead-letter-routing-key", RoutingKeyNames.DLQ_POST_CREATED_ROUTING_KEY }
                    }
                );

                // Bind queue to exchange with routing key
                _channel.QueueBind(
                    queue: queueName,
                    exchange: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                    routingKey: RoutingKeyNames.POST_CREATED_ROUTING_KEY,
                    arguments: null
                );

                _logger.LogInformation("Analytics consumer service started and listening for post created events");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting analytics consumer service");
                throw;
            }

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("Channel is null, cannot start consuming messages");
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<PostCreatedMessage>(body);

                    if (message != null)
                    {
                        await ProcessPostCreatedMessageAsync(message);
                        _channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation($"Successfully processed post created event for PostId: {message.PostId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing post created message");
                    // Reject and requeue for retry
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: "analytics_post_created_queue",
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        private async Task ProcessPostCreatedMessageAsync(PostCreatedMessage message)
        {
            using var scope = _serviceProvider.CreateScope();
            var postAnalyticsDao = scope.ServiceProvider.GetRequiredService<IPostAnalyticsDao>();
            var regionMetricsDao = scope.ServiceProvider.GetRequiredService<IRegionMetricsDao>();

            // Save post analytics
            var postAnalytics = new PostAnalytics
            {
                Id = Guid.NewGuid(),
                PostId = message.PostId,
                Content = message.Content,
                Category = message.Category,
                UserId = message.UserId,
                CreatedAt = message.CreatedAt,
                Latitude = message.Latitude,
                Longitude = message.Longitude,
                City = message.City,
                State = message.State,
                Country = message.Country,
                LikesCount = message.LikesCount,
                CommentsCount = message.CommentsCount,
                RelevanceScore = message.RelevanceScore
            };

            await postAnalyticsDao.AddAsync(postAnalytics);

            // Update region metrics if location is available
            if (!string.IsNullOrEmpty(message.City) || !string.IsNullOrEmpty(message.State))
            {
                await UpdateRegionMetricsAsync(regionMetricsDao, postAnalytics);
            }
        }

        private async Task UpdateRegionMetricsAsync(IRegionMetricsDao regionMetricsDao, PostAnalytics postAnalytics)
        {
            var regionIdentifier = $"{postAnalytics.City ?? "Unknown"}-{postAnalytics.State ?? "Unknown"}";
            var regionMetrics = await regionMetricsDao.FindByRegionIdentifierAsync(regionIdentifier);

            if (regionMetrics == null)
            {
                // Create new region metrics
                regionMetrics = new RegionMetrics
                {
                    Id = Guid.NewGuid(),
                    RegionIdentifier = regionIdentifier,
                    City = postAnalytics.City,
                    State = postAnalytics.State,
                    Country = postAnalytics.Country,
                    TotalPosts = 1,
                    PostsByCategory = new Dictionary<GeoCidadao.Models.Enums.PostCategory, int> { { postAnalytics.Category, 1 } },
                    LastUpdated = DateTime.UtcNow,
                    MostFrequentCategory = postAnalytics.Category,
                    MostFrequentCategoryCount = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await regionMetricsDao.AddAsync(regionMetrics);
            }
            else
            {
                // Update existing region metrics
                regionMetrics.TotalPosts++;
                
                if (regionMetrics.PostsByCategory.ContainsKey(postAnalytics.Category))
                {
                    regionMetrics.PostsByCategory[postAnalytics.Category]++;
                }
                else
                {
                    regionMetrics.PostsByCategory[postAnalytics.Category] = 1;
                }

                // Update most frequent category
                var maxCategory = regionMetrics.PostsByCategory.OrderByDescending(kvp => kvp.Value).First();
                regionMetrics.MostFrequentCategory = maxCategory.Key;
                regionMetrics.MostFrequentCategoryCount = maxCategory.Value;
                regionMetrics.LastUpdated = DateTime.UtcNow;
                regionMetrics.UpdatedAt = DateTime.UtcNow;

                await regionMetricsDao.UpdateAsync(regionMetrics);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("Analytics consumer service stopped");
            
            return base.StopAsync(cancellationToken);
        }
    }
}
