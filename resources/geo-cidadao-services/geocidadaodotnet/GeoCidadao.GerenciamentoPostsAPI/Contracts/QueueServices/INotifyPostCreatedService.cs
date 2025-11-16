namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices
{
    public interface INotifyPostCreatedService
    {
        void NotifyPostCreated(Guid postId);
    }
}
