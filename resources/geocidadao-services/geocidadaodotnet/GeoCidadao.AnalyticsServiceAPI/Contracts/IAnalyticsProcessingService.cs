using GeoCidadao.AMQP.Messages;

namespace GeoCidadao.AnalyticsServiceAPI.Contracts
{
    public interface IAnalyticsProcessingService
    {
        Task ProcessPostAnalyticsEventAsync(PostAnalyticsMessage message);
    }
}
