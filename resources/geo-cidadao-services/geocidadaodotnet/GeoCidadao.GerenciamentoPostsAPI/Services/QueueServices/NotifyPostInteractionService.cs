using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using RabbitMQ.Client;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.QueueServices
{
    public class NotifyPostInteractionService : RabbitMQPublisherService, INotifyPostInteractionService
    {

        public NotifyPostInteractionService(ILogger<NotifyPostInteractionService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            ConfigureExchange(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                exchangeType: ExchangeType.Topic,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME
            );
        }

        public void NotifyPostInteraction(Guid postId, string interactionType, Guid userId) =>
            PublishMessage(
                new PostInteractionMessage()
                {
                    PostId = postId,
                    InteractionType = interactionType,
                    UserId = userId
                },
                exchange: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                routingKey: RoutingKeyNames.POST_INTERACTION_ROUTING_KEY
            );
    }
}
