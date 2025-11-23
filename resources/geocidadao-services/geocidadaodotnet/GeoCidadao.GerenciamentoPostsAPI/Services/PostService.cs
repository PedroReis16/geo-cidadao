using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using NetTopologySuite.Geometries;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Nominatim;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.ConnectionServices;
using GeoCidadao.Models.OAuth;
using GeoCidadao.OAuth.Models;
using GeoCidadao.GerenciamentoPostsAPI.Config;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class PostService(
        ILogger<PostService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory scopeFactory,
        IPostDao postDao,
        IPostLocationDao postLocationDao
        ) : IPostService
    {
        private readonly ILogger<PostService> _logger = logger;
        private readonly HttpContext? _context = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IPostDao _postDao = postDao;
        private readonly IPostLocationDao _postLocationDao = postLocationDao;


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
        public async Task<PostDTO> CreatePostAsync(NewPostDTO newPost)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();

            RequestUser requestUser = _context!.User.GetUserDetails();
            List<string> mediaUrls = new();

            try
            {
                Post newPostEntity = new()
                {
                    Content = newPost.Content,
                    UserId = requestUser.Id,
                };

                if (newPost.HasPosition)
                {
                    INominatimService nominatimService = scope.ServiceProvider.GetRequiredService<INominatimService>();

                    AddressDTO? addressInfo = await nominatimService.GetCoordinatesDetailsAsync(newPost.Latitude!.Value, newPost.Longitude!.Value);

                    PostLocation newLocation = new()
                    {
                        Post = newPostEntity,
                        Location = new Point(newPost.Longitude!.Value, newPost.Latitude!.Value) { SRID = 4326 },
                        Address = addressInfo.Road,
                        City = addressInfo.City,
                        State = addressInfo.State,
                        Country = addressInfo.Country,
                        Suburb = addressInfo.Suburb,
                    };
                    newPostEntity.Location = newLocation;
                }

                if (newPost.MediaFiles.Any())
                {
                    string baseUrl = scope.ServiceProvider.GetRequiredService<IConfiguration>().GetValue<string>(AppSettingsProperties.ServicePath)!;

                    IPostMediaService postMediaService = scope.ServiceProvider.GetRequiredService<IPostMediaService>();

                    List<PostMedia> postMedias = await postMediaService.UploadPostMediasAsync(newPostEntity.Id, newPost.MediaFiles);

                    newPostEntity.Medias = postMedias.Select(x => { x.Post = newPostEntity; return x; }).ToList();

                    mediaUrls = newPostEntity.Medias.Select(m => $"{baseUrl}/posts/{newPostEntity.Id}/media/{m.Id}").ToList();
                }

                await _postDao.AddAsync(newPostEntity);

                _ = Task.Run(() =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();
                    notifyService.NotifyNewPost(new()
                    {
                        Id = newPostEntity.Id,
                        PostOwnerId = newPostEntity.UserId,
                        Content = newPostEntity.Content,
                        City = newPostEntity.Location?.City,
                        Latitude = newPostEntity.Location?.Location.Y,
                        Longitude = newPostEntity.Location?.Location.X,
                        AuthorName = $"{requestUser.FirstName} {requestUser.LastName}".Trim(),
                        AuthorUsername = requestUser.Email,
                        AuthorProfilePicture = requestUser.Picture,
                        MediaUrls = mediaUrls
                    });
                });

                return new(newPostEntity);
            }
            catch (EntityValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Um erro ocorreu ao tentar criar o post para o usuário '{requestUser.Id}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.UserId, requestUser.Id    },
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

                _ = Task.Run(() =>
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();

                    notifyService.NotifyPostChanged(postId);
                });
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
                        catch (Exception) { /* Ignorar erros de deleção de mídia */ }
                    })
                );

                _ = Task.Run(() =>
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();

                    notifyService.NotifyPostDeleted(postId);
                });
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

    }
}