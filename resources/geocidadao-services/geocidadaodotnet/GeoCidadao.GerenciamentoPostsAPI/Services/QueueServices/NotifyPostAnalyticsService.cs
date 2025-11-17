using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using RabbitMQ.Client;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.QueueServices
{
    public class NotifyPostAnalyticsService : RabbitMQPublisherService, INotifyPostAnalyticsService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotifyPostAnalyticsService> _logger;

        public NotifyPostAnalyticsService(
            ILogger<NotifyPostAnalyticsService> logger, 
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory) : base(logger, configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            
            ConfigureExchange(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                exchangeType: ExchangeType.Topic,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME
            );
        }

        public async Task NotifyPostAnalyticsAsync(Guid postId)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var postDao = scope.ServiceProvider.GetRequiredService<IPostDao>();
                var postLocationDao = scope.ServiceProvider.GetRequiredService<IPostLocationDao>();

                // Get post data
                var post = await postDao.FindAsync(postId);
                if (post == null)
                {
                    _logger.LogWarning($"Post {postId} not found for analytics notification");
                    return;
                }

                // Get location data
                var location = await postLocationDao.GetPostLocationByPostIdAsync(postId);
                if (location == null)
                {
                    _logger.LogDebug($"Post {postId} has no location, skipping analytics notification");
                    return;
                }

                // Build analytics message with enriched data
                var analyticsMessage = new PostAnalyticsMessage
                {
                    PostId = post.Id,
                    Title = post.Content.Length > 100 ? post.Content.Substring(0, 100) : post.Content,
                    Description = post.Content,
                    Category = post.Category,
                    Latitude = location.Position.Y,
                    Longitude = location.Position.X,
                    EventTimestamp = DateTime.UtcNow,
                    LikesCount = post.LikesCount,
                    CommentsCount = post.CommentsCount,
                    RelevanceScore = post.RelevanceScore
                };

                // Publish to analytics queue
                PublishMessage(
                    analyticsMessage, 
                    exchange: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME, 
                    routingKey: RoutingKeyNames.POST_ANALYTICS_ROUTING_KEY
                );

                _logger.LogInformation($"Analytics event published for post {postId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing analytics event for post {postId}: {ex.Message}");
                // Don't throw - analytics failures shouldn't break post creation
            }
        }
    }
}
