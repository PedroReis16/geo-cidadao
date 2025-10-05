using RabbitMQ.Client.Events;

namespace GeoCidadao.AMQP.Contracts;

public interface ISubscriberService : IQueueService
{
    void ConsumeQueue(
        string exchangeName, 
        string queueName,
        string routingKey,
        EventHandler<BasicDeliverEventArgs> onNewMessage,
        string? dlqExchangeName = null,
        string? dlqQueueName = null,
        string? dlqRoutingKeyName = null,
        int deliveryLimit = 3);
}
