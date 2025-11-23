using GeoCidadao.Caching.Services;
using GeoCidadao.Database.Cache;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Cache
{
    internal class PostLocationDaoCache(InMemoryCacheService cacheService) : RepositoryCache<PostLocation>(cacheService), IPostLocationDaoCache
    {

    }
}