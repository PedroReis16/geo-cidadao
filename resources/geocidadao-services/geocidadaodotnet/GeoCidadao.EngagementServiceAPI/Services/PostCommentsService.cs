using GeoCidadao.EngagementServiceAPI.Contracts;
using GeoCidadao.EngagementServiceAPI.Contracts.ConnectionServices;
using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.EngagementServiceAPI.Models.DTOs.Comments;
using GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.EngagementServiceAPI;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Quartz.Logging;

namespace GeoCidadao.EngagementServiceAPI.Services
{
    public class PostCommentsService(ILogger<PostCommentsService> logger, IHttpContextAccessor contextAccessor, IServiceScopeFactory scopeFactory) : IPostCommentsService
    {
        private readonly ILogger<PostCommentsService> _logger = logger;
        private readonly HttpContext? _httpContext = contextAccessor.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task<List<CommentDTO>?> GetPostCommentsAsync(Guid postId, int? itemsCount, int? pageNumber, CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();
                IUserManagementService userManagementService = scope.ServiceProvider.GetRequiredService<IUserManagementService>();

                List<PostComment> postComments = await postCommentsDao.GetPostCommentsAsync(postId, itemsCount, pageNumber);

                if (postComments.Count == 0)
                    return null;

                List<Task<UserDTO?>> profileTasks = postComments
                    .DistinctBy(pc => pc.UserId)
                    .Select(pc => pc.UserId)
                    .Select(async userId =>
                    {
                        using IServiceScope profileQueryScope = _scopeFactory.CreateScope();
                        IUserManagementService userManagementService = profileQueryScope.ServiceProvider.GetRequiredService<IUserManagementService>();

                        UserDTO? userProfile = await userManagementService.GetUserProfileAsync(userId, cancellationToken);

                        return userProfile;
                    })
                    .ToList();

                await Task.WhenAll(profileTasks);

                Dictionary<Guid, UserDTO?> userProfiles = profileTasks
                    .Select(t => t.Result)
                    .Where(profile => profile != null)
                    .ToDictionary(profile => profile!.Id, profile => profile);

                List<CommentDTO> comments = postComments.
                    Select(pc =>
                    {
                        UserDTO? userProfile = userProfiles.ContainsKey(pc.UserId) ? userProfiles[pc.UserId] : null;

                        //TODO: Verificar o formato de contagem/retorno dos likes contidos no comentário para talvez, uma consulta que retorne de forma mais eficiente
                        return new CommentDTO
                        {
                            Id = pc.Id,
                            Author = userProfile ?? new(),
                            Content = pc.Content,
                            CreatedAt = pc.CreatedAt,
                            LikesCount = pc.Likes.Count
                        };
                    })
                    .ToList();

                return comments;
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                _logger.LogError(errorMessage, ex, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
                throw new Exception(errorMessage, ex);
            }
        }
        public async Task<CommentDTO> AddPostCommentAsync(Guid postId, Guid userId, NewCommentDTO newComment, CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();

                PostComment postComment = new()
                {
                    PostId = postId,
                    Content = newComment.Content,
                    UserId = userId,
                };

                await postCommentsDao.AddAsync(postComment);

                NotifyUserInterection(postId, InteractionType.PostComment);

                CommentDTO createdComment = new()
                {
                    Id = postComment.Id,
                    Author = new UserDTO { Id = userId },
                    Content = postComment.Content,
                    CreatedAt = postComment.CreatedAt,
                    LikesCount = 0
                };
                return createdComment;
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao tentar adicionar um novo comentário ao post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId },
                    { LogConstants.UserId, userId }
                });
                throw new Exception(errorMessage, ex);
            }
        }

        public async Task<CommentDTO> UpdatePostCommentAsync(Guid postId, Guid commentId, UpdateCommentDTO updatedComment, CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();

                PostComment? existingComment = await postCommentsDao.FindAsync(commentId);

                if (existingComment == null || existingComment.PostId != postId)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não encontrado para o post informado", ErrorCodes.POST_COMMENT_NOT_FOUND);

                existingComment.Content = updatedComment.Content;

                await postCommentsDao.UpdateAsync(existingComment);

                CommentDTO commentDto = new()
                {
                    Id = existingComment.Id,
                    Author = new UserDTO { Id = existingComment.UserId },
                    Content = existingComment.Content,
                    CreatedAt = existingComment.CreatedAt,
                    LikesCount = 0
                };

                _logger.LogInformation($"O comentário '{commentId}' do post '{postId}' foi atualizado com sucesso.", null, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId },
                    {LogConstants.OldEntity, existingComment.Content },
                    {LogConstants.UpdatedEntity, updatedComment.Content }
                });

                return commentDto;
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao tentar atualizar o comentário '{commentId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
                throw new Exception(errorMessage, ex);
            }
        }

        public async Task DeletePostCommentAsync(Guid postId, Guid commentId, CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();

                PostComment? existingComment = await postCommentsDao.FindAsync(commentId);

                if (existingComment == null || existingComment.PostId != postId)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não encontrado para o post informado", ErrorCodes.POST_COMMENT_NOT_FOUND);

                await postCommentsDao.DeleteAsync(existingComment);

                NotifyUserInterection(postId, InteractionType.PostCommentDeleted);

                _logger.LogInformation($"O comentário '{commentId}' do post '{postId}' foi deletado com sucesso.", null, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao tentar deletar o comentário '{commentId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
                throw new Exception(errorMessage, ex);
            }
        }

        public async Task LikePostCommentAsync(Guid postId, Guid commentId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();
                ICommentLikesDao commentLikesDao = scope.ServiceProvider.GetRequiredService<ICommentLikesDao>();

                PostComment? trackedComment = await postCommentsDao.FindAsync(postId, commentId, true);

                if (trackedComment == null || trackedComment.PostId != postId)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não encontrado para o post informado", ErrorCodes.POST_COMMENT_NOT_FOUND);

                CommentLike commentLike = new()
                {
                    Id = commentId,
                    UserId = userId,
                    Comment = trackedComment
                };

                await commentLikesDao.AddAsync(commentLike);

                NotifyUserInterection(postId, InteractionType.PostCommentLiked);

                _logger.LogInformation($"O comentário '{commentId}' do post '{postId}' foi curtido com sucesso pelo usuário '{userId}'.", null, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao tentar dar like no comentário '{commentId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
                throw new Exception(errorMessage, ex);
            }
        }

        public async Task UnlikePostCommentAsync(Guid postId, Guid commentId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostCommentsDao postCommentsDao = scope.ServiceProvider.GetRequiredService<IPostCommentsDao>();
                ICommentLikesDao commentLikesDao = scope.ServiceProvider.GetRequiredService<ICommentLikesDao>();

                PostComment? trackedComment = await postCommentsDao.FindAsync(postId, commentId, false);

                if (trackedComment == null || trackedComment.PostId != postId)
                    throw new EntityValidationException(nameof(PostComment), "Comentário não encontrado para o post informado", ErrorCodes.POST_COMMENT_NOT_FOUND);

                CommentLike commentLike = new()
                {
                    Id = commentId,
                    UserId = userId,
                    Comment = trackedComment
                };

                await commentLikesDao.DeleteAsync(commentLike);

                NotifyUserInterection(postId, InteractionType.PostCommentDeleted);

                _logger.LogInformation($"O like do comentário '{commentId}' do post '{postId}' foi removido com sucesso pelo usuário '{userId}'.", null, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao tentar remover o like do comentário '{commentId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex, _httpContext, new Dictionary<string, object?>
                {
                    { LogConstants.EntityId, postId }
                });
                throw new Exception(errorMessage, ex);
            }

        }


        private void NotifyUserInterection(Guid postId, InteractionType interactionType)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostInteraction notifyPostInteraction = scope.ServiceProvider.GetRequiredService<INotifyPostInteraction>();

            notifyPostInteraction.NotifyPostInteraction(postId, interactionType);
        }

    }
}