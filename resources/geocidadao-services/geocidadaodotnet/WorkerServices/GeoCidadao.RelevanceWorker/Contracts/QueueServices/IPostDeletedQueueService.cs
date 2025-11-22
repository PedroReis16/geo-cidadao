namespace GeoCidadao.RelevanceWorker.Contracts.QueueServices
{
    public interface IPostDeletedQueueService
    {
        void ConsumeQueue();
    }
}