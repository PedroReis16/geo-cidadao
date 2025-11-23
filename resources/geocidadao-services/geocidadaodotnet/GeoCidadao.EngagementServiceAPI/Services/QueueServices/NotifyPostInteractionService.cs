using GeoCidadao.AMQP.Configuration;
using GeoCidadao.AMQP.Messages;
using GeoCidadao.AMQP.Services;
using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.EngagementServiceAPI.Models.DTOs;
using GeoCidadao.Models.Enums;
using RabbitMQ.Client;

namespace GeoCidadao.EngagementServiceAPI.Services.QueueServices
{
    public class NotifyPostInteractionService : RabbitMQPublisherService, INotifyPostInteraction
    {
        public NotifyPostInteractionService(ILogger<NotifyPostInteractionService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            ConfigureExchange(
                exchangeName: ExchangeNames.POST_ENGAGEMENT_TOPIC_EXCHANGE,
                exchangeType: ExchangeType.Topic,
                dlqExchangeName: ExchangeNames.DLQ_POST_ENGAGEMENT_TOPIC_EXCHANGE
            );
        }

        public void NotifyPostInteraction(Guid postId, InteractionType interactionType) =>
            PublishMessage(new PostInteractionMessage()
            {
                PostId = postId,
                InteractionType = interactionType
            }, exchange: ExchangeNames.POST_ENGAGEMENT_TOPIC_EXCHANGE, routingKey: RoutingKeyNames.POST_INTERACTED_ROUTING_KEY);

        public void NotifyPostReported(Guid postId, Guid reporterUserId, DelationDTO postReport)
        {
            // Notify the specific service about the new post report
            PublishMessage(new NewPostReportMessage()
            {
                PostId = postId,
                ReporterUserId = reporterUserId,
                Reason = postReport.ReasonDetails,
                ReportType = postReport.Type
            }, exchange: ExchangeNames.POST_ENGAGEMENT_TOPIC_EXCHANGE, routingKey: RoutingKeyNames.POST_REPORTED_ROUTING_KEY);


            // Notify the relevance worker to evaluate the post's relevance
            NotifyPostInteraction(postId, InteractionType.PostReported);
        }
    }
}