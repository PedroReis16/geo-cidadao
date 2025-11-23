using GeoCidadao.Models.Enums;

namespace GeoCidadao.RelevanceWorker.Contracts
{
    public interface IInteractionService
    {
        Task UpdatePostInteractionAsync(Guid postId, InteractionType interactionType);
    }
}