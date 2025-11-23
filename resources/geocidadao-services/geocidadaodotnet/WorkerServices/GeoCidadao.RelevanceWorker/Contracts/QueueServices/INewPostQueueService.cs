namespace GeoCidadao.RelevanceWorker.Contracts.QueueServices
{
    public interface INewPostQueueService
    {
        void ConsumeQueue();
    }
}