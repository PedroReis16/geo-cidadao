using GeoCidadao.Database;
using GeoCidadao.Database.CacheContracts;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostLikeDao(GeoDbContext context) : BaseDao<PostLike>(context), IPostLikeDao
    {
        protected override IRepositoryCache<PostLike>? GetCache() => null;

        protected override Task ValidateEntityForInsert(params PostLike[] obj)
        {
            foreach (PostLike like in obj)
            {
                if (like.UserId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostLike), "A curtida deve estar associada a um usuário.", ErrorCodes.USER_ID_REQUIRED);
                if (like.PostId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostLike), "A curtida deve estar associada a um post.", ErrorCodes.POST_NOT_FOUND);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostLike[] obj)
        {
            return Task.CompletedTask;
        }

        public async Task<PostLike?> GetLikeByUserAndPostAsync(Guid userId, Guid postId)
        {
            return await _context.Set<PostLike>()
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public async Task<List<PostLike>> GetPostLikesAsync(Guid postId, int? itemsCount = null, int? pageNumber = null)
        {
            IQueryable<PostLike> query = _context.Set<PostLike>()
                .Where(l => l.PostId == postId)
                .OrderByDescending(l => l.CreatedAt);

            if (itemsCount.HasValue && pageNumber.HasValue && itemsCount > 0 && pageNumber > 0)
            {
                int skip = (pageNumber.Value - 1) * itemsCount.Value;
                query = query.Skip(skip).Take(itemsCount.Value);
            }
            else if (itemsCount.HasValue && itemsCount > 0)
            {
                query = query.Take(itemsCount.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetPostLikesCountAsync(Guid postId)
        {
            return await _context.Set<PostLike>()
                .CountAsync(l => l.PostId == postId);
        }
    }
}
