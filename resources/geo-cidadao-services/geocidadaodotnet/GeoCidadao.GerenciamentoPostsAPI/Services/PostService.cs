using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Model.Constants;
using GeoCidadao.Model.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Model.Enums;
using GeoCidadao.Model.Exceptions;
using GeoCidadao.Model.Extensions;
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


        public Task<PostDTO?> GetPostAsync(Guid postId)
        {
            throw new NotImplementedException();
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


                if (newPost.Media != null && newPost.Media.Any())
                {
                    if (newPost.Media.Count > 10)
                        throw new EntityValidationException(nameof(Post), "Um post não pode conter mais do que 10 mídias.", ErrorCodes.POST_MEDIA_LIMIT_EXCEEDED);

                    IMediaService mediaService = scope.ServiceProvider.GetRequiredService<IMediaService>();
                    List<Task> uploadTasks = new();

                    newPostEntity.Medias = new();

                    for (int i = 0; i < newPost.Media.Count; i++)
                    {
                        PostMedia media = new()
                        {
                            Order = i,
                        };
                        Stream fileContent = newPost.Media[i].OpenReadStream();

                        uploadTasks.Add(Task.Run(async () =>
                        {
                            await mediaService.UploadMediaAsync(postId, media.Id, fileContent, out string fileExtension);
                            media.MediaType = fileExtension;
                            media.FileSize = fileContent.Length;
                            newPostEntity.Medias.Add(media);
                        }));
                    }

                    await Task.WhenAll(uploadTasks);
                }

                await postRepository.AddAsync(newPostEntity);

                return new();
            }
            catch (EntityValidationException)
            {
                _ = DeleteErrorPostMediaAsync(postId);
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
                _ = DeleteErrorPostMediaAsync(postId);

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