using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.EFDao
{
    public class PostLikesDao(GeoDbContext context, IPostLikeDaoCache? cache = null) : BaseDao<PostLike>(context, cache), IPostLikesDao
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

        public Task RemovePostLikeAsync(Guid postId, Guid userId)
        {
            PostLike? trackedLike = _context.Set<PostLike>().FirstOrDefault(pl => pl.PostId == postId && pl.UserId == userId);

            if (trackedLike != null)
            {
                _context.Set<PostLike>().Remove(trackedLike);
                return _context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

    }
}