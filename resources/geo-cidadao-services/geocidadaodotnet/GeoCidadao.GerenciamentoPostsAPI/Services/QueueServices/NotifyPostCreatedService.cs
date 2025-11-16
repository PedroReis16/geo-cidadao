using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using RabbitMQ.Client;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.QueueServices
{
    public class NotifyPostCreatedService : RabbitMQPublisherService, INotifyPostCreatedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotifyPostCreatedService(
            ILogger<NotifyPostCreatedService> logger, 
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory) : base(logger, configuration)
        {
            _scopeFactory = scopeFactory;
            ConfigureExchange(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                exchangeType: ExchangeType.Topic,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME
            );
        }

        public async void NotifyPostCreated(Guid postId)
        {
            using var scope = _scopeFactory.CreateScope();
            var postDao = scope.ServiceProvider.GetRequiredService<IPostDao>();
            var postLocationDao = scope.ServiceProvider.GetRequiredService<IPostLocationDao>();

            var post = await postDao.FindAsync(postId);
            if (post == null)
            {
                Logger.LogWarning($"Post {postId} not found, cannot send analytics event");
                return;
            }

            var postLocation = await postLocationDao.GetPostLocationByPostIdAsync(postId);
            
            var message = new PostCreatedMessage
            {
                PostId = post.Id,
                Content = post.Content,
                Category = post.Category,
                UserId = post.UserId,
                CreatedAt = post.CreatedAt,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                RelevanceScore = post.RelevanceScore
            };

            if (postLocation != null)
            {
                message.Latitude = postLocation.Position.Y;
                message.Longitude = postLocation.Position.X;
            }

            PublishMessage(
                message,
                exchange: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                routingKey: RoutingKeyNames.POST_CREATED_ROUTING_KEY
            );
        }
    }
}
