using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class PostService(
        ILogger<PostService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory scopeFactory,
        IPostDao postDao
        ) : IPostService
    {
        private readonly ILogger<PostService> _logger = logger;
        private readonly HttpContext? _context = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IPostDao _postDao = postDao;


        public async Task<PostDTO?> GetPostAsync(Guid postId)
        {
            try
            {
                Post? post = await _postDao.FindAsync(postId);

                if (post == null)
                    return null;

                return new PostDTO(post);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar obter o post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task<List<PostDTO>> GetUserPostsAsync(Guid userId, int? itemsCount = null, int? pageNumber = null)
        {
            try
            {
                List<Post> posts = await _postDao.GetUserPostsAsync(userId, itemsCount, pageNumber);

                return posts.Select(p => new PostDTO(p)).ToList();
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar obter os posts do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.UserId, userId },
                });
                throw new Exception(errorMsg, ex);
            }
        }
        public async Task<PostDTO> CreatePostAsync(Guid userId, NewPostDTO newPost)
        {
            Guid postId = Guid.NewGuid();

            try
            {
                Post newPostEntity = new()
                {
                    Id = postId,
                    Content = newPost.Content,
                    UserId = userId
                };


                await _postDao.AddAsync(newPostEntity);

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

        public async Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost)
        {
            try
            {
                Post? existingPost = await _postDao.FindAsync(postId);

                if (existingPost == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado para atualização", ErrorCodes.POST_NOT_FOUND);

                if (!string.IsNullOrEmpty(updatedPost.Content))
                    existingPost.Content = updatedPost.Content;

                // TODO: Adicionar a validação de alteração de localização do post

                await _postDao.UpdateAsync(existingPost);
                
                NotifyPostChanged(postId);    
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Um erro ocorreu ao tentar atualizar o post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task DeletePostAsync(Guid postId)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostDao postRepository = scope.ServiceProvider.GetRequiredService<IPostDao>();

                Post? postToDelete = await _postDao.FindAsync(postId);

                if (postToDelete == null)
                    throw new EntityValidationException(nameof(Post), "Post não encontrado para deleção", ErrorCodes.POST_NOT_FOUND);

                //TODO: Implementar a validação de propriedade do post antes de deletar

                await Task.WhenAll(
                    postRepository.DeleteAsync(postToDelete),
                    Task.Run(async () =>
                    {
                        try
                        {
                            IPostMediaService postMediaService = scope.ServiceProvider.GetRequiredService<IPostMediaService>();
                            await postMediaService.DeletePostMediasAsync(postId);
                        }
                        catch(Exception){ /* Ignorar erros de deleção de mídia */ }
                    })
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