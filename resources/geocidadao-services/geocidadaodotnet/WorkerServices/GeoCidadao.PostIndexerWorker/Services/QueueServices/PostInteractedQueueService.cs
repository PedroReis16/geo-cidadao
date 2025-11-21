using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Services;
using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using RabbitMQ.Client.Events;

namespace GeoCidadao.PostIndexerWorker.Services.QueueServices
{
    public class PostInteractedQueueService(ILogger<PostInteractedQueueService> logger, IConfiguration configuration) : RabbitMQSubscriberService(logger, configuration), IPostInterectedQueueService
    {
        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.RELEVANCE_WORKER_DIRECT_EXCHANGE_NAME,
                queueName: QueueNames.POST_INDEXER_WORKER_RELEVANCE_POST_QUEUE_NAME,
                routingKey: RoutingKeyNames.POST_INTERACTED_ROUTING_KEY,
                onNewMessage: OnPostInteracted,
                dlqExchangeName: ExchangeNames.DLQ_RELEVANCE_WORKER_DIRECT_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_POST_INDEXER_WORKER_RELEVANCE_POST_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_INTERACTED_ROUTING_KEY
            );
        }

        private void OnPostInteracted(object? sender, BasicDeliverEventArgs e)
        {
            
        }
    }
}