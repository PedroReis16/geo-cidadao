using GeoCidadao.Caching.Contracts;
using GeoCidadao.Database.Cache;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.Cache
{
    public class CommentLikeDaoCache(IInMemoryCacheService cacheService) : RepositoryCache<CommentLike>(cacheService), ICommentLikeDaoCache
    {

    }
}