using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.EngagementServiceAPI.Database.EFDao
{
    public class PostCommentsDao(GeoDbContext context, IPostCommentsDaoCache? cache = null) : BaseDao<PostComment>(context, cache), IPostCommentsDao
    {
        protected override IPostCommentsDaoCache? GetCache() => _cache as IPostCommentsDaoCache;

        protected override Task ValidateEntityForInsert(params PostComment[] obj)
        {
            foreach (PostComment comment in obj)
            {
                if (string.IsNullOrWhiteSpace(comment.Content))
                    throw new EntityValidationException(nameof(PostComment), "O conteúdo do comentário não pode ser nulo ou vazio", ErrorCodes.INVALID_POST_COMMENT);
                if (comment.Content.Length > 5000)
                    throw new EntityValidationException(nameof(PostComment), "O conteúdo do comentário não pode exceder 5000 caracteres", ErrorCodes.INVALID_POST_COMMENT_LENGTH);
                if (comment.PostId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostComment), "O ID do post associado ao comentário é inválido", ErrorCodes.INVALID_POST_ID);
                if (comment.UserId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostComment), "O ID do usuário associado ao comentário é inválido", ErrorCodes.INVALID_USER_ID);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostComment[] obj)
        {
            return ValidateEntityForInsert(obj);
        }

        public async Task<PostComment?> FindAsync(Guid postId, Guid commentId, bool track = false)
        {
            PostComment? result = null;
            var cache = GetCache() as IPostCommentsDaoCache;

            if (cache != null && !track)
            {
                result = cache.GetPostComment(commentId, postId);
                if (result != null)
                    return result;
            }

            IQueryable<PostComment> query = _context.Set<PostComment>()
                .Where(pc => pc.Id == commentId && pc.PostId == postId);
            if (!track)
                query = query.AsNoTracking();

            result = await query.FirstOrDefaultAsync();

            if (result != null && !track)
                cache?.AddEntity(result);
            return result;
        }

        public Task<List<PostComment>> GetPostCommentsAsync(Guid postId, int? itemsCount, int? pageNumber)
        {
            IQueryable<PostComment> query = _context.Set<PostComment>()
                .Where(pc => pc.PostId == postId)
                .Include(pc => pc.Likes)
                .OrderByDescending(pc => pc.CreatedAt);

            if (itemsCount.HasValue && itemsCount.Value > 1 && pageNumber.HasValue && pageNumber.Value > 1)
                query = query
                    .Skip(itemsCount.Value * (pageNumber.Value - 1))
                    .Take(itemsCount.Value);

            return query.AsNoTracking().ToListAsync();
        }

        public Task DeletePostCommentsAsync(Guid postId)
        {
            return _context.Set<PostComment>()
                .Where(pc => pc.PostId == postId)
                .ExecuteDeleteAsync();
        }

        public Task RemoveUserCommentsAsync(Guid userId)
        {
            return _context.Set<PostComment>()
                .Where(pc => pc.UserId == userId)
                .ExecuteDeleteAsync();
        }
    }

}