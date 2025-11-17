using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.OAuth.Contracts
{
    public interface IResourceFetcher<TResource>
    {
        Task<TResource?> GetAsync(ActionContext ctx, CancellationToken ct = default);
    }
}