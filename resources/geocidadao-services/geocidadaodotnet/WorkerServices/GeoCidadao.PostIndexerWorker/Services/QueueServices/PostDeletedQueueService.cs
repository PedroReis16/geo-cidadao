using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Services;
using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using RabbitMQ.Client.Events;

namespace GeoCidadao.PostIndexerWorker.Services.QueueServices
{
    public class PostDeletedQueueService(ILogger<PostDeletedQueueService> logger, IConfiguration configuration) : RabbitMQSubscriberService(logger, configuration), IPostDeletedQueueService
    {
        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.POST_INDEXER_WORKER_DELETED_POST_QUEUE_NAME,
                routingKey: RoutingKeyNames.POST_DELETED_ROUTING_KEY,
                onNewMessage: OnPostDeleted,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_POST_INDEXER_WORKER_DELETED_POST_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_DELETED_ROUTING_KEY
            );
        }

        private void OnPostDeleted(object? sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}