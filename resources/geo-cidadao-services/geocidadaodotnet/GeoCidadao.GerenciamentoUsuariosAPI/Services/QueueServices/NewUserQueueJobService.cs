using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.Model.Helpers;
using RabbitMQ.Client.Events;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.QueueServices
{
    public class NewUserQueueJobService(ILogger<NewUserQueueJobService> logger, IConfiguration configuration) : RabbitMQSubscriberService(logger, configuration), INewUserQueueJobService
    {
        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.KEYCLOAK_EVENTS_TOPIC_EXCHANGE,
                queueName: QueueNames.USER_MANAGEMENT_NEW_USER_QUEUE_NAME,
                routingKey: RoutingKeyNames.NEW_USER_ROUTING_KEY,
                onNewMessage: OnNewUser,
                dlqExchangeName: ExchangeNames.DLQ_KEYCLOAK_EVENTS_TOPIC_EXCHANGE,
                dlqQueueName: QueueNames.DLQ_USER_MANAGEMENT_NEW_USER_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_NEW_USER_ROUTING_KEY
            );
        }

        private void OnNewUser(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var newUserMessageBytes = e.Body.ToArray();
                NewUserMessage? message = JsonSerializer.Deserialize<NewUserMessage>(newUserMessageBytes, options: new() { PropertyNameCaseInsensitive = true });

                if (message != null)
                {

                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Não foi possível processar a mensagem de novo usuário recebida: {ex.GetFullMessage()}");
                Channel?.BasicNack(e.DeliveryTag, false, true);
            }
        }
    }
}