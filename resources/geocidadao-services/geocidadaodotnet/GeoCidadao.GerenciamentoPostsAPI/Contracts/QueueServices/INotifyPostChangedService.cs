
using GeoCidadao.AMQP.Messages;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices
{
    public interface INotifyPostChangedService
    {
        void NotifyNewPost(NewPostMessage newPostMessage);
        void NotifyPostChanged(Guid postId);
        void NotifyPostDelete(Guid postId);
    }
}