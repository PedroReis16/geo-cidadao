
namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices
{
    public interface INotifyPostChangedService
    {
        void NotifyPostChanged(Guid postId);
    }
}