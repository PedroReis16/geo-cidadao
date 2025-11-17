using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using RabbitMQ.Client;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.QueueServices
{
    public class NotifyPostChangedService : RabbitMQPublisherService, INotifyPostChangedService
    {

        public NotifyPostChangedService(ILogger<NotifyPostChangedService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            ConfigureExchange(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                exchangeType: ExchangeType.Topic,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME
            );
        }

        public void NotifyPostChanged(Guid postId) => PublishMessage(new PostChangedMessage() { PostId = postId }, exchange: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME, routingKey: RoutingKeyNames.POST_CHANGED_ROUTING_KEY);
    }
}