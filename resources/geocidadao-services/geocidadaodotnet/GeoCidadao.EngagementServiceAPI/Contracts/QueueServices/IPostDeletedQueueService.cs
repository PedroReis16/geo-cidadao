namespace GeoCidadao.EngagementServiceAPI.Contracts.QueueServices
{
    public interface IPostDeletedQueueService
    {
        void ConsumeQueue();
    }
}