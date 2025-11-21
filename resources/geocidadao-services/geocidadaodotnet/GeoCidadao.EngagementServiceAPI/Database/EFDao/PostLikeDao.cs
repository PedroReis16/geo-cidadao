using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.EFDao
{
    public class PostLikeDao(GeoDbContext context, IPostLikeDaoCache? cache = null) : BaseDao<PostLike>(context, cache), IPostLikesDao
    {
        protected override IPostLikeDaoCache? GetCache() => _cache as IPostLikeDaoCache;

        protected override Task ValidateEntityForInsert(params PostLike[] obj)
        {
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostLike[] obj)
        {
            return Task.CompletedTask;
        }
    }
}