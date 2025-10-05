using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GeoCidadao.AMQP.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeoCidadao.AMQP.Services;

public class RabbitMQSubscriberService(ILogger<RabbitMQQueueService> logger, IConfiguration configuration) : RabbitMQQueueService(logger, configuration), ISubscriberService
{
    public void ConsumeQueue(
        string exchangeName,
        string queueName,
        string routingKey,
        EventHandler<BasicDeliverEventArgs> onNewMessage,
        string? dlqExchangeName = null,
        string? dlqQueueName = null,
        string? dlqRoutingKeyName = null,
        int deliveryLimit = 2)
    {
        try
        {
            Channel?.ExchangeDeclarePassive(exchange: exchangeName);

            var arguments =
                string.IsNullOrWhiteSpace(dlqExchangeName) || string.IsNullOrWhiteSpace(dlqRoutingKeyName) ? null :
                new Dictionary<string, object>
                {
                    { "x-queue-type", "quorum" },
                    { "x-dead-letter-exchange", dlqExchangeName },
                    { "x-dead-letter-routing-key", dlqRoutingKeyName },
                    { "x-delivery-limit", deliveryLimit }
                };

            if (arguments != null)
            {
                Channel?.QueueDeclare(
                    queue: dlqQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Channel?.QueueBind(
                    queue: dlqQueueName,
                    exchange: dlqExchangeName,
                    routingKey: dlqRoutingKeyName
                );
            }

            Channel?.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
            );

            Channel?.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey
            );

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += onNewMessage;

            Channel?.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );
        }
        catch (Exception ex)
        {
            Logger.LogError($"Erro ao configurar Subscriber no RabbitMQ: {ex.Message}", ex);
            throw;
        }
    }
}
