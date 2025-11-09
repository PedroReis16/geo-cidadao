using GeoCidadao.Models.Entities;
using GeoCidadao.OAuth.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.OAuth.Middlewares
{
    public class DelegateFetcher<TEntity> : IResourceFetcher<TEntity> where TEntity : BaseEntity
    {
        private readonly Func<ActionContext, CancellationToken, Task<TEntity?>> _fetcher;

        public DelegateFetcher(Func<ActionContext, CancellationToken, Task<TEntity?>> fetcher)
        {
            _fetcher = fetcher;
        }

        public Task<TEntity?> GetAsync(ActionContext ctx, CancellationToken ct = default) => _fetcher(ctx, ct);
    }
}