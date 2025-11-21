namespace GeoCidadao.PostIndexerWorker.Contracts.QueueServices
{
    public interface INewPostQueueService
    {
        void ConsumeQueue();
    }
}