using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.Models.Extensions;
using GeoCidadao.RelevanceWorker.Contracts;
using GeoCidadao.RelevanceWorker.Contracts.QueueServices;
using RabbitMQ.Client.Events;

namespace GeoCidadao.RelevanceWorker.Services.QueueServices
{
    public class PostDeletedQueueService(ILogger<PostDeletedQueueService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), IPostDeletedQueueService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.RELEVANCE_WORKER_DELETED_POST_QUEUE_NAME,
                routingKey: RoutingKeyNames.POST_DELETED_ROUTING_KEY,
                onNewMessage: OnPostDeleted,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_RELEVANCE_WORKER_DELETED_POST_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_DELETED_ROUTING_KEY
            );
        }

        private void OnPostDeleted(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var byteMessage = e.Body.ToArray();
                PostChangedMessage? message = JsonSerializer.Deserialize<PostChangedMessage>(byteMessage);

                if (message?.PostId != null)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    IElasticSearchService indexService = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();

                    indexService.DeletePostIndexAsync(message.PostId).GetAwaiter().GetResult();

                    Logger.LogInformation($"A solicitação de remoção do post '{message.PostId}' foi processada com sucesso");
                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Houve um erro ao tentar processar a mensagem de posts deletados: {ex.GetFullMessage()}");
                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}