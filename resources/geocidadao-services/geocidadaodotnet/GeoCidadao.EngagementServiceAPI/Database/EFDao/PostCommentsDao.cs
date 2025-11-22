using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.EngagementServiceAPI.Database.EFDao
{
    public class PostCommentsDao(GeoDbContext context, IPostCommentsDaoCache? cache = null) : BaseDao<PostComment>(context, cache), IPostCommentsDao
    {
        protected override IPostCommentsDaoCache? GetCache() => _cache as IPostCommentsDaoCache;

        protected override Task ValidateEntityForInsert(params PostComment[] obj)
        {
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostComment[] obj)
        {
            return Task.CompletedTask;
        }


        public Task DeletePostCommentsAsync(Guid postId)
        {
            return _context.Set<PostComment>()
                .Where(pc => pc.PostId == postId)
                .ExecuteDeleteAsync();
        }

    }

}