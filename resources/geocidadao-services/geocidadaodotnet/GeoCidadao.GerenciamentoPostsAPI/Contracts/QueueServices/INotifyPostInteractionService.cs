
namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices
{
    public interface INotifyPostInteractionService
    {
        void NotifyPostInteraction(Guid postId, string interactionType, Guid userId);
    }
}
