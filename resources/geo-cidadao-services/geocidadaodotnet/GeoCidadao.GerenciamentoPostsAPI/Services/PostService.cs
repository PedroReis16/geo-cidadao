using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Model.Constants;
using GeoCidadao.Model.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Model.Enums;
using GeoCidadao.Model.Exceptions;
using GeoCidadao.Model.Extensions;
using GeoCidadao.Model.OAuth;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class PostService(
        ILogger<PostService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory scopeFactory
        ) : IPostService
    {
        private readonly ILogger<PostService> _logger = logger;
        private readonly HttpContext? _context = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;


        public async Task<PostDTO?> GetPostAsync(Guid postId)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();

                IPostDao postRepository = scope.ServiceProvider.GetRequiredService<IPostDao>();

                Post? post = await postRepository.FindAsync(postId);

                if (post == null)
                    return null;

                return new PostDTO(post);
            }
            catch(Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar obter o post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public Task<List<PostDTO>> GetUserPostsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
        public async Task<PostDTO> CreatePostAsync(Guid userId, NewPostDTO newPost)
        {
            Guid postId = Guid.NewGuid();

            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();

                IPostDao postRepository = scope.ServiceProvider.GetRequiredService<IPostDao>();

                Post newPostEntity = new()
                {
                    Id = postId,
                    Content = newPost.Content,
                    UserId = userId
                };


                await postRepository.AddAsync(newPostEntity);

                return new(newPostEntity);
            }
            catch (EntityValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Um erro ocorreu ao tentar criar o post para o usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.UserId, userId },
                    { LogConstants.EntityId, postId },
                });


                throw new Exception(errorMsg, ex);
            }
        }

        public async Task DeleteErrorPostMediaAsync(Guid postId)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IMediaService mediaService = scope.ServiceProvider.GetRequiredService<IMediaService>();

                List<string> postMediaKeys = await mediaService.GetPostMediaKeysAsync(postId);

                List<Task> deleteTasks = new();
                foreach (string mediaKey in postMediaKeys.Where(k => k.StartsWith(postId.ToString())))
                {
                    deleteTasks.Add(mediaService.DeleteMediaAsync(mediaKey));
                }

                await Task.WhenAll(deleteTasks);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar deletar as mídias do post '{postId}' após falha na criação do post: {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
            }
        }

        public async Task UploadPostMediaAsync(Guid postId, IFormFile mediaFile)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();

                Guid userId = _context!.User.GetUserId();

                Post? post = await scope.ServiceProvider.GetRequiredService<IPostDao>().FindAsync(postId);

                if (post == null)
                    throw new EntityValidationException(nameof(Post), $"Post '{postId}' não encontrado para adiçao de mídia", ErrorCodes.POST_NOT_FOUND);

                if (post.UserId != userId)
                {
                    if (!_context.User.Identities.Any(i => i.IsAuthenticated && i.HasClaim(c => c.Type == "role" && c.Value == "admin")))
                        throw new UnauthorizedAccessException($"Usuário '{userId}' não é proprietário do post '{postId}'");
                }




                _logger.LogInformation($"O post '{postId}' será atualizado com nova mídia pelo usuário '{userId}'");
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar adicionar mídia ao post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost)
        {
            //TODO: Implementar a validação de propriedade do post antes de deletar
            throw new NotImplementedException();
        }

        public async Task DeletePostAsync(Guid userId, Guid postId)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostDao postRepository = scope.ServiceProvider.GetRequiredService<IPostDao>();

                Post? postToDelete = await postRepository.FindAsync(postId);

                if (postToDelete == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado para deleção", ErrorCodes.POST_NOT_FOUND);

                //TODO: Implementar a validação de propriedade do post antes de deletar

                await Task.WhenAll(
                    postRepository.DeleteAsync(postToDelete),
                    DeleteErrorPostMediaAsync(postId)
                );

                NotifyPostChanged(postId);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar deletar o post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        private void NotifyPostChanged(Guid postId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();

            notifyService.NotifyPostChanged(postId);
        }

    }
}