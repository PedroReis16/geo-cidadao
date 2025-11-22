using GeoCidadao.RelevanceWorker.Models.DTOs;

namespace GeoCidadao.RelevanceWorker.Contracts
{
    public interface IElasticSearchService
    {
        Task IndexPostAsync(Guid postId, RelevanceDocument postDocument, CancellationToken cancellationToken = default);
        Task DeletePostIndexAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<RelevanceDocument?> FindPostDetailsAsync(Guid postId);
        Task UpdatePostAsync(Guid postId, RelevanceDocument postDocument, CancellationToken cancellationToken = default);
    }
}