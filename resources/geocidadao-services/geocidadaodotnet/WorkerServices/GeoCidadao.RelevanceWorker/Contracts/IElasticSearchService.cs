using GeoCidadao.RelevanceWorker.Models.DTOs;

namespace GeoCidadao.RelevanceWorker.Contracts
{
    public interface IElasticSearchService
    {
        Task IndexPostAsync(PostDocument postDocument, CancellationToken cancellationToken = default);
        Task DeletePostIndexAsync(Guid postId, CancellationToken cancellationToken = default);
    }
}