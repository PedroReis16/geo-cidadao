using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.Models.Extensions;
using RabbitMQ.Client.Events;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.QueueServices
{
    public class UserDeletedQueueService(ILogger<UserDeletedQueueService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), IUserDeletedQueueService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.POST_MANAGEMENT_USER_DELETED_QUEUE_NAME,
                routingKey: RoutingKeyNames.USER_DELETED_ROUTING_KEY,
                onNewMessage: OnUserDeleted,
                dlqExchangeName: ExchangeNames.DLQ_USER_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_POST_MANAGEMENT_USER_DELETED_QUEUE_NAME,
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
                    IUserPostService userPostService = scope.ServiceProvider.GetRequiredService<IUserPostService>();

                    userPostService.RemoveAllUserContentAsync(message.UserId).GetAwaiter().GetResult();

                    Logger.LogInformation($"Foram deletados posts do usuário de ID {message.UserId}.");
                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Houve um erro ao tentar processar a mensagem de perfis de usuários removidos: {ex.GetFullMessage()}");
                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}