using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.RelevanceWorker.Contracts;
using GeoCidadao.RelevanceWorker.Contracts.QueueServices;
using RabbitMQ.Client.Events;

namespace GeoCidadao.RelevanceWorker.Services.QueueServices
{
    public class PostInteractQueueService(ILogger<PostInteractQueueService> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) : RabbitMQSubscriberService(logger, configuration), IPostInteractQueueService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 5);

        public void ConsumeQueue()
        {
            try
            {
                _semaphore.Wait();

                ConsumeQueue(
                    exchangeName: ExchangeNames.POST_ENGAGEMENT_TOPIC_EXCHANGE,
                    queueName: QueueNames.RELEVANCE_WORKER_POST_INTERACT_QUEUE,
                    routingKey: RoutingKeyNames.POST_INTERACTED_ROUTING_KEY,
                    onNewMessage: OnNewInteract,
                    dlqExchangeName: ExchangeNames.DLQ_POST_ENGAGEMENT_TOPIC_EXCHANGE,
                    dlqQueueName: QueueNames.DLQ_RELEVANCE_WORKER_POST_INTERACT_QUEUE,
                    dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_INTERACTED_ROUTING_KEY
                );
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void OnNewInteract(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var messageBody = e.Body.ToArray();
                PostInteractionMessage? message = JsonSerializer.Deserialize<PostInteractionMessage>(messageBody);

                if (message != null)
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    IInteractionService interactionService = scope.ServiceProvider.GetRequiredService<IInteractionService>();

                    interactionService.UpdatePostInteractionAsync(message.PostId, message.InteractionType).GetAwaiter().GetResult();

                    Logger.LogInformation("Mensagem de interação de post recebida: {@Message}", message);
                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao processar mensagem de interação de post.");
                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}