using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.EFDao
{
    public class CommentLikesDao(GeoDbContext context, ICommentLikesDaoCache? cache = null) : BaseDao<CommentLike>(context, cache), ICommentLikesDao
    {
        protected override ICommentLikesDaoCache? GetCache() => _cache as ICommentLikesDaoCache;

        protected override Task ValidateEntityForInsert(params CommentLike[] obj)
        {
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params CommentLike[] obj)
        {
            return Task.CompletedTask;
        }
    }
}