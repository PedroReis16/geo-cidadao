using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using RabbitMQ.Client;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.QueueServices
{
    internal class NotifyUserChangedService : RabbitMQPublisherService, INotifyUserChangedService
    {
        public NotifyUserChangedService(ILogger<NotifyUserChangedService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            ConfigureExchange(
                exchangeName: ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                exchangeType: ExchangeType.Topic,
                dlqExchangeName: ExchangeNames.DLQ_USER_MANAGEMENT_TOPIC_EXCHANGE_NAME
            );
        }

        public void NotifyUserChanged(Guid userId) =>
            PublishMessage(new UserChangedMessage() { UserId = userId }, ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME, RoutingKeyNames.USER_CHANGED_ACTIONS_ROUTING_KEY);

        public void NotifyUserDeleted(Guid userId) =>
            PublishMessage(new UserChangedMessage() { UserId = userId }, ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME, RoutingKeyNames.USER_DELETED_ROUTING_KEY);
    }
}