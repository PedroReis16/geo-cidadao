using System.Text.Json;
using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Extensions;
using RabbitMQ.Client.Events;

namespace GeoCidadao.EngagementServiceAPI.Services.QueueServices
{
    public class PostDeletedQueueService(ILogger<PostDeletedQueueService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) : RabbitMQSubscriberService(logger, configuration), IPostDeletedQueueService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void ConsumeQueue()
        {
            ConsumeQueue(
                exchangeName: ExchangeNames.POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                queueName: QueueNames.ENGAGEMENT_SERVICE_POST_DELETED_QUEUE_NAME,
                routingKey: RoutingKeyNames.POST_DELETED_ROUTING_KEY,
                onNewMessage: OnDeletedPost,
                dlqExchangeName: ExchangeNames.DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME,
                dlqQueueName: QueueNames.DLQ_ENGAGEMENT_SERVICE_POST_DELETED_QUEUE_NAME,
                dlqRoutingKeyName: RoutingKeyNames.DLQ_POST_DELETED_ROUTING_KEY
            );
        }

        private void OnDeletedPost(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var messageBody = e.Body.ToArray();

                PostChangedMessage? message = JsonSerializer.Deserialize<PostChangedMessage>(messageBody);

                if (message != null)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();

                    IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();
                    IPostLikesDao postLikesDao = scope.ServiceProvider.GetRequiredService<IPostLikesDao>();

                    postCommentsDao.DeletePostCommentsAsync(message.PostId).GetAwaiter().GetResult();
                    postLikesDao.DeletePostLikesAsync(message.PostId).GetAwaiter().GetResult();

                    Logger.LogInformation($"Os registros relacionados ao post '{message.PostId}' foram removidos com sucesso.");
                }
                Channel?.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao processar a mensagem da fila de exclusão de post: {ex.GetFullMessage()}";
                Logger.LogError(errorMessage, ex);
                Channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}