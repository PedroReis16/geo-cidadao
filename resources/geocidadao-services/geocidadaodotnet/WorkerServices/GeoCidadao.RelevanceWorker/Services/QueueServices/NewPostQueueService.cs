using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.Models.Extensions;
using GeoCidadao.RelevanceWorker.Contracts;
using GeoCidadao.RelevanceWorker.Contracts.QueueServices;
using GeoCidadao.RelevanceWorker.Models.DTOs;
using RabbitMQ.Client.Events;

namespace GeoCidadao.RelevanceWorker.Services.QueueServices
{
    public class NewPostQueueService(ILogger<NewPostQueueService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), INewPostQueueService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public void ConsumeQueue()
        {
            try
            {
                _semaphore.Wait();

                ConsumeQueue(
                    exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                    queueName: QueueNames.RELEVANCE_WORKER_NEW_POST_QUEUE_NAME,
                    routingKey: RoutingKeyNames.NEW_POST_ROUTING_KEY,
                    onNewMessage: OnNewPost,
                    dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                    dlqQueueName: QueueNames.DLQ_RELEVANCE_WORKER_NEW_POST_QUEUE_NAME,
                    dlqRoutingKeyName: RoutingKeyNames.DLQ_NEW_POST_ROUTING_KEY
                );
            }
            finally
            {
                _semaphore.Release();
            }
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

                    RelevanceDocument newPost = new()
                    {
                        RelevanceScore = 1.0,
                        LikesCount = 0,
                        CommentsCount = 0
                    };

                    _ = service.IndexPostAsync(message.Id, newPost);

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