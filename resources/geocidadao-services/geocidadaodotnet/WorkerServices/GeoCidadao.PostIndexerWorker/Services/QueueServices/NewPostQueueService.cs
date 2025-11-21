using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.Models.Extensions;
using GeoCidadao.PostIndexerWorker.Contracts;
using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;
using GeoCidadao.PostIndexerWorker.Models.DTOs;
using RabbitMQ.Client.Events;

namespace GeoCidadao.PostIndexerWorker.Services.QueueServices
{
    public class NewPostQueueService(ILogger<NewPostQueueService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), INewPostQueueService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.POST_INDEXER_WORKER_NEW_POST_QUEUE_NAME,
                routingKey: RoutingKeyNames.NEW_POST_ROUTING_KEY,
                onNewMessage: OnNewPost,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_POST_INDEXER_WORKER_NEW_POST_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_NEW_POST_ROUTING_KEY
            );
        }

        private void OnNewPost(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var newPostMessageBytes = e.Body.ToArray();
                NewPostMessage? message = JsonSerializer.Deserialize<NewPostMessage>(newPostMessageBytes, options: new() { PropertyNameCaseInsensitive = true });

                if (message != null)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    IElasticSearchService service = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();

                    PostDocument newPost = new()
                    {
                        Id = message.Id,
                        PostOwnerId = message.PostOwnerId,
                        Content = message.Content,
                        City = message.City,
                        Latitude = message.Latitude,
                        Longitude = message.Longitude,
                        Tags = message.Tags
                    };

                    _ = service.IndexPostAsync(newPost);

                    Logger.LogInformation($"O post '{message.Id}' foi recebido e processado com sucesso pela fila de novos posts.");
                }

                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Não foi possível processar a mensagem de novo post: {ex.GetFullMessage()}");

                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}