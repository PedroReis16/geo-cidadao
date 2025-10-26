using GeoCidadao.Caching.Services;
using GeoCidadao.Database.Cache;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.Model.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Cache
{
    internal class PostMediaDaoCache(InMemoryCacheService cacheService) : RepositoryCache<PostMedia>(cacheService), IPostMediaDaoCache
    {

    }
}