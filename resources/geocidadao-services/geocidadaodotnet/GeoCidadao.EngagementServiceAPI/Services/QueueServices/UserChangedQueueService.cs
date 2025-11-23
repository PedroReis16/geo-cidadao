using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.EngagementServiceAPI.Contracts.CacheServices;
using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Extensions;
using RabbitMQ.Client.Events;

namespace GeoCidadao.EngagementServiceAPI.Services.QueueServices
{
    public class UserChangedQueueService(ILogger<UserChangedQueueService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), IUserChangedQueueService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.ENGAGEMENT_SERVICE_USER_CHANGED_QUEUE_NAME,
                routingKey: RoutingKeyNames.USER_CHANGED_ACTIONS_ROUTING_KEY,
                onNewMessage: OnUserChanged,
                dlqExchangeName: ExchangeNames.DLQ_USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_ENGAGEMENT_SERVICE_USER_CHANGED_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_USER_CHANGED_ACTIONS_ROUTING_KEY
            );
        }

        private void OnUserChanged(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var byteMessage = e.Body.ToArray();
                UserChangedMessage? message = JsonSerializer.Deserialize<UserChangedMessage>(byteMessage);

                if (message != null)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();

                    IUserCacheService cacheService = scope.ServiceProvider.GetRequiredService<IUserCacheService>();
                    cacheService.RemoveUser(message.UserId);

                    Logger.LogInformation($"Mensagem de usuário alterado processada com sucesso para o usuário {message.UserId}.");
                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao processar a mensagem de usuário alterado na fila de mensagens. Detalhes: {ex.GetFullMessage()}";
                Logger.LogError(errorMessage, ex);
                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }

        public void ConsumeUserDeletedQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.ENGAGEMENT_SERVICE_USER_DELETED_QUEUE_NAME,
                routingKey: RoutingKeyNames.USER_DELETED_ROUTING_KEY,
                onNewMessage: OnUserDeleted,
                dlqExchangeName: ExchangeNames.DLQ_USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_ENGAGEMENT_SERVICE_USER_DELETED_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_USER_DELETED_ROUTING_KEY
            );
        }

        private void OnUserDeleted(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var byteMessage = e.Body.ToArray();
                UserChangedMessage? message = JsonSerializer.Deserialize<UserChangedMessage>(byteMessage);

                if (message != null)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();

                    IUserCacheService cacheService = scope.ServiceProvider.GetRequiredService<IUserCacheService>();
                    cacheService.RemoveUser(message.UserId);

                    IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();
                    postCommentsDao.RemoveUserCommentsAsync(message.UserId);

                    Logger.LogInformation($"Mensagem de exclusão de perfil de usuário processada com sucesso para o usuário {message.UserId}.");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao processar a mensagem de exclusão do perfil de usuário: {ex.GetFullMessage()}";
                Logger.LogError(errorMessage, ex);
                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}