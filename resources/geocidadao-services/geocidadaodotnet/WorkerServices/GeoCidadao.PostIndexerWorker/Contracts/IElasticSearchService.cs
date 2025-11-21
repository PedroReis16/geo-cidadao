using GeoCidadao.PostIndexerWorker.Models.DTOs;

namespace GeoCidadao.PostIndexerWorker.Contracts
{
    public interface IElasticSearchService
    {
        Task IndexPostAsync(PostDocument postDocument, CancellationToken cancellationToken = default);
        Task UpdatePostIndexAsync(UpdatedPostDocument updatedPostDocument, CancellationToken cancellationToken = default);
        Task DeletePostIndexAsync(Guid postId, CancellationToken cancellationToken = default);
    }
}