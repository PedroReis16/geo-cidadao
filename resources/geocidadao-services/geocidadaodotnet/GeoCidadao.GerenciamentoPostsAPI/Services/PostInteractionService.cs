using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class PostInteractionService(
        ILogger<PostInteractionService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory scopeFactory,
        IPostDao postDao,
        IPostLikeDao postLikeDao,
        IPostCommentDao postCommentDao
        ) : IPostInteractionService
    {
        private readonly ILogger<PostInteractionService> _logger = logger;
        private readonly HttpContext? _context = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IPostDao _postDao = postDao;
        private readonly IPostLikeDao _postLikeDao = postLikeDao;
        private readonly IPostCommentDao _postCommentDao = postCommentDao;

        public async Task<PostLikeDTO> LikePostAsync(Guid postId, Guid userId)
        {
            try
            {
                // Verify post exists
                Post? post = await _postDao.FindAsync(postId);
                if (post == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado", ErrorCodes.POST_NOT_FOUND);

                // Check if user already liked this post
                PostLike? existingLike = await _postLikeDao.GetLikeByUserAndPostAsync(userId, postId);
                if (existingLike != null)
                    throw new EntityValidationException(nameof(PostLike), "Usuário já curtiu este post", ErrorCodes.DUPLICATE_LIKE);

                // Create new like
                PostLike newLike = new()
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = userId
                };

                await _postLikeDao.AddAsync(newLike);

                // Update post metrics
                post.LikesCount++;
                await UpdatePostRelevanceScore(post);
                await _postDao.UpdateAsync(post);

                // Notify interaction
                NotifyPostInteraction(postId, "like", userId);

                return new PostLikeDTO(newLike);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar curtir o post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                    { LogConstants.UserId, userId }
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task UnlikePostAsync(Guid postId, Guid userId)
        {
            try
            {
                // Verify post exists
                Post? post = await _postDao.FindAsync(postId);
                if (post == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado", ErrorCodes.POST_NOT_FOUND);

                // Check if like exists
                PostLike? like = await _postLikeDao.GetLikeByUserAndPostAsync(userId, postId);
                if (like == null)
                    throw new EntityValidationException(nameof(PostLike), "Curtida não encontrada", ErrorCodes.LIKE_NOT_FOUND);

                await _postLikeDao.DeleteAsync(like);

                // Update post metrics
                post.LikesCount = Math.Max(0, post.LikesCount - 1);
                await UpdatePostRelevanceScore(post);
                await _postDao.UpdateAsync(post);

                // Notify interaction
                NotifyPostInteraction(postId, "unlike", userId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar remover curtida do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                    { LogConstants.UserId, userId }
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task<PostCommentDTO> CreateCommentAsync(Guid postId, Guid userId, CreateCommentDTO createCommentDto)
        {
            try
            {
                // Verify post exists
                Post? post = await _postDao.FindAsync(postId);
                if (post == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado", ErrorCodes.POST_NOT_FOUND);

                // Create new comment
                PostComment newComment = new()
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = userId,
                    Content = createCommentDto.Content.Trim()
                };

                await _postCommentDao.AddAsync(newComment);

                // Update post metrics
                post.CommentsCount++;
                await UpdatePostRelevanceScore(post);
                await _postDao.UpdateAsync(post);

                // Notify interaction
                NotifyPostInteraction(postId, "comment", userId);

                return new PostCommentDTO(newComment);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar criar comentário no post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                    { LogConstants.UserId, userId }
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task<PostCommentDTO> UpdateCommentAsync(Guid postId, Guid commentId, Guid userId, UpdateCommentDTO updateCommentDto)
        {
            try
            {
                // Verify post exists
                Post? post = await _postDao.FindAsync(postId);
                if (post == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado", ErrorCodes.POST_NOT_FOUND);

                // Get comment
                PostComment? comment = await _postCommentDao.FindAsync(commentId);
                if (comment == null)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não encontrado", ErrorCodes.COMMENT_NOT_FOUND);

                // Verify comment belongs to this post
                if (comment.PostId != postId)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não pertence a este post", ErrorCodes.COMMENT_NOT_FOUND);

                // Verify user is the owner
                if (comment.UserId != userId)
                    throw new EntityValidationException(nameof(PostComment), "Apenas o autor pode editar o comentário", ErrorCodes.UNAUTHORIZED_COMMENT_EDIT);

                // Update comment
                comment.Content = updateCommentDto.Content.Trim();
                comment.UpdatedAt = DateTime.UtcNow;

                await _postCommentDao.UpdateAsync(comment);

                // Notify interaction
                NotifyPostInteraction(postId, "comment_edit", userId);

                return new PostCommentDTO(comment);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar atualizar comentário '{commentId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                    { LogConstants.UserId, userId }
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task DeleteCommentAsync(Guid postId, Guid commentId, Guid userId, bool isModerator = false)
        {
            try
            {
                // Verify post exists
                Post? post = await _postDao.FindAsync(postId);
                if (post == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado", ErrorCodes.POST_NOT_FOUND);

                // Get comment
                PostComment? comment = await _postCommentDao.FindAsync(commentId);
                if (comment == null)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não encontrado", ErrorCodes.COMMENT_NOT_FOUND);

                // Verify comment belongs to this post
                if (comment.PostId != postId)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não pertence a este post", ErrorCodes.COMMENT_NOT_FOUND);

                // Verify user is the owner or moderator
                if (!isModerator && comment.UserId != userId)
                    throw new EntityValidationException(nameof(PostComment), "Apenas o autor ou moderadores podem excluir o comentário", ErrorCodes.UNAUTHORIZED_COMMENT_DELETE);

                await _postCommentDao.DeleteAsync(comment);

                // Update post metrics
                post.CommentsCount = Math.Max(0, post.CommentsCount - 1);
                await UpdatePostRelevanceScore(post);
                await _postDao.UpdateAsync(post);

                // Notify interaction
                NotifyPostInteraction(postId, "comment_delete", userId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar excluir comentário '{commentId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                    { LogConstants.UserId, userId }
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task<List<PostCommentDTO>> GetPostCommentsAsync(Guid postId, int? itemsCount = null, int? pageNumber = null)
        {
            try
            {
                List<PostComment> comments = await _postCommentDao.GetPostCommentsAsync(postId, itemsCount, pageNumber);
                return comments.Select(c => new PostCommentDTO(c)).ToList();
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar obter comentários do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId }
                });
                throw new Exception(errorMsg, ex);
            }
        }

        private async Task UpdatePostRelevanceScore(Post post)
        {
            // Simple relevance calculation: likes + (comments * 2)
            // Comments are weighted more as they represent higher engagement
            // This can be enhanced with time decay, category weights, etc.
            post.RelevanceScore = post.LikesCount + (post.CommentsCount * 2.0);
            
            await Task.CompletedTask;
        }

        private void NotifyPostInteraction(Guid postId, string interactionType, Guid userId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostInteractionService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostInteractionService>();

            notifyService.NotifyPostInteraction(postId, interactionType, userId);
        }
    }
}
