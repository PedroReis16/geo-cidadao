namespace GeoCidadao.PostIndexerWorker.Contracts.QueueServices
{
    public interface IPostDeletedQueueService
    {
        void ConsumeQueue();
    }
}