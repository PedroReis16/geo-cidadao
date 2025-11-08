using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Model.Constants;
using GeoCidadao.Model.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Model.Exceptions;
using GeoCidadao.Model.Extensions;

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

        public Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost)
        {
            //TODO: Implementar a validação de propriedade do post antes de deletar
            throw new NotImplementedException();
        }

        public async Task DeletePostAsync(Guid userId)
        {
            // try
            // {
            //     using IServiceScope scope = _scopeFactory.CreateScope();
            //     IPostDao postRepository = scope.ServiceProvider.GetRequiredService<IPostDao>();

            //     // Post? postToDelete = await postRepository.FindAsync(postId);

            //     if (postToDelete == null)
            //         throw new EntityValidationException(nameof(Post), "Post não encontrado para deleção", ErrorCodes.POST_NOT_FOUND);

            //     //TODO: Implementar a validação de propriedade do post antes de deletar

            //     await Task.WhenAll(
            //         postRepository.DeleteAsync(postToDelete),
            //         DeleteErrorPostMediaAsync(postId)
            //     );

            //     NotifyPostChanged(postId);
            // }
            // catch (Exception ex)
            // {
            //     string errorMsg = $"Ocorreu um erro ao tentar deletar o post '{postId}': {ex.GetFullMessage()}";
            //     _logger.LogError(ex, errorMsg, _context, new()
            //     {
            //         { LogConstants.EntityId, postId },
            //     });
            //     throw new Exception(errorMsg, ex);
            // }
        }

        private void NotifyPostChanged(Guid postId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();

            notifyService.NotifyPostChanged(postId);
        }

    }
}