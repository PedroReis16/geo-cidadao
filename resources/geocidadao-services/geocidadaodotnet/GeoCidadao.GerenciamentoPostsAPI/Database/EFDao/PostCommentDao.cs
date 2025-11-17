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
    internal class PostCommentDao(GeoDbContext context) : BaseDao<PostComment>(context), IPostCommentDao
    {
        protected override IRepositoryCache<PostComment>? GetCache() => null;

        protected override Task ValidateEntityForInsert(params PostComment[] obj)
        {
            foreach (PostComment comment in obj)
            {
                if (comment.UserId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostComment), "O comentário deve estar associado a um usuário.", ErrorCodes.USER_ID_REQUIRED);
                if (comment.PostId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostComment), "O comentário deve estar associado a um post.", ErrorCodes.POST_NOT_FOUND);
                if (string.IsNullOrWhiteSpace(comment.Content))
                    throw new EntityValidationException(nameof(PostComment), "O conteúdo do comentário é obrigatório.", ErrorCodes.COMMENT_CONTENT_REQUIRED);
                if (comment.Content.Length > 500)
                    throw new EntityValidationException(nameof(PostComment), "O conteúdo do comentário não pode exceder 500 caracteres.", ErrorCodes.COMMENT_CONTENT_TOO_LONG);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostComment[] obj)
        {
            foreach (PostComment comment in obj)
            {
                if (string.IsNullOrWhiteSpace(comment.Content))
                    throw new EntityValidationException(nameof(PostComment), "O conteúdo do comentário é obrigatório.", ErrorCodes.COMMENT_CONTENT_REQUIRED);
                if (comment.Content.Length > 500)
                    throw new EntityValidationException(nameof(PostComment), "O conteúdo do comentário não pode exceder 500 caracteres.", ErrorCodes.COMMENT_CONTENT_TOO_LONG);
            }
            return Task.CompletedTask;
        }

        public async Task<List<PostComment>> GetPostCommentsAsync(Guid postId, int? itemsCount = null, int? pageNumber = null)
        {
            IQueryable<PostComment> query = _context.Set<PostComment>()
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt);

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

        public async Task<int> GetPostCommentsCountAsync(Guid postId)
        {
            return await _context.Set<PostComment>()
                .CountAsync(c => c.PostId == postId);
        }
    }
}
