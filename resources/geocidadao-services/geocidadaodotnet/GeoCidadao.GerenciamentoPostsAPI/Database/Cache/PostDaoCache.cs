using GeoCidadao.Caching.Services;
using GeoCidadao.Database.Cache;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Cache
{
    internal class PostDaoCache(InMemoryCacheService cacheService) : RepositoryCache<Post>(cacheService), IPostDaoCache
    {

    }
}