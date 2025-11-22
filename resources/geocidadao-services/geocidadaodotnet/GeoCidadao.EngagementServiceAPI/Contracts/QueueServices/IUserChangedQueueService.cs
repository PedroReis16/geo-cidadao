namespace GeoCidadao.EngagementServiceAPI.Contracts.QueueServices
{
    public interface IUserChangedQueueService
    {
        void ConsumeQueue();
        void ConsumeUserDeletedQueue();
    }
}