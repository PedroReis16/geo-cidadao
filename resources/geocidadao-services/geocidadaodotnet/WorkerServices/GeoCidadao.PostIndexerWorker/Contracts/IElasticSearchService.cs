using GeoCidadao.PostIndexerWorker.Models.DTOs;

namespace GeoCidadao.PostIndexerWorker.Contracts
{
    public interface IElasticSearchService
    {
        Task IndexPostAsync(Guid postId, PostDocument postDocument, CancellationToken cancellationToken = default);
        Task DeletePostIndexAsync(Guid postId, CancellationToken cancellationToken = default);
    }
}