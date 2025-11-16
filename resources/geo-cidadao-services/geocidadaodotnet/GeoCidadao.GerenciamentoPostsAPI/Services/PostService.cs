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

        public async Task<PostWithLocationDTO?> GetPostWithLocationAsync(Guid postId)
        {
            try
            {
                Post? post = await _postDao.FindAsync(postId);

                if (post == null)
                    return null;

                PostLocation? location = await _postLocationDao.GetPostLocationByPostIdAsync(postId);
                PostLocationDTO? locationDto = null;

                if (location != null)
                {
                    locationDto = new PostLocationDTO
                    {
                        Latitude = location.Position.Y,
                        Longitude = location.Position.X
                    };
                }

                return new PostWithLocationDTO(post, locationDto);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar obter o post com localização '{postId}': {ex.GetFullMessage()}";
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

        public async Task<List<PostWithLocationDTO>> GetPostsByLocationAsync(LocationQueryDTO locationQuery)
        {
            try
            {
                List<PostLocation> postLocations;

                // Query by coordinates and radius
                if (locationQuery.Latitude.HasValue && locationQuery.Longitude.HasValue && locationQuery.RadiusKm.HasValue)
                {
                    var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                    Point center = geometryFactory.CreatePoint(new Coordinate(locationQuery.Longitude.Value, locationQuery.Latitude.Value));
                    
                    postLocations = await _postLocationDao.GetPostsWithinRadiusAsync(
                        center, 
                        locationQuery.RadiusKm.Value,
                        locationQuery.ItemsCount,
                        locationQuery.PageNumber
                    );
                }
                // Query by city/state
                else if (!string.IsNullOrEmpty(locationQuery.City) || !string.IsNullOrEmpty(locationQuery.State))
                {
                    postLocations = await _postLocationDao.GetPostsByLocationAsync(
                        locationQuery.City,
                        locationQuery.State,
                        locationQuery.Country,
                        locationQuery.ItemsCount,
                        locationQuery.PageNumber
                    );
                }
                else
                {
                    // No location parameters provided - return empty list
                    return new List<PostWithLocationDTO>();
                }

                // Fetch the actual posts and order by relevance
                var postIds = postLocations.Select(pl => pl.PostId).ToList();
                var posts = new List<PostWithLocationDTO>();

                foreach (var postLocation in postLocations)
                {
                    var post = await _postDao.FindAsync(postLocation.PostId);
                    if (post != null)
                    {
                        var locationDto = new PostLocationDTO
                        {
                            Latitude = postLocation.Position.Y,
                            Longitude = postLocation.Position.X
                        };
                        posts.Add(new PostWithLocationDTO(post, locationDto));
                    }
                }

                // Sort by relevance score (higher engagement) while maintaining proximity as secondary factor
                return posts.OrderByDescending(p => p.RelevanceScore).ThenBy(p => p.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar obter posts por localização: {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context);
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

                // Save location if provided
                if (newPost.Position != null && 
                    !string.IsNullOrEmpty(newPost.Position.Latitude) && 
                    !string.IsNullOrEmpty(newPost.Position.Longitude))
                {
                    try
                    {
                        if (double.TryParse(newPost.Position.Latitude, out double lat) &&
                            double.TryParse(newPost.Position.Longitude, out double lon))
                        {
                            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                            Point position = geometryFactory.CreatePoint(new Coordinate(lon, lat));

                            PostLocation postLocation = new()
                            {
                                Id = Guid.NewGuid(),
                                PostId = postId,
                                Position = position,
                                Category = newPostEntity.Category
                            };

                            await _postLocationDao.AddAsync(postLocation);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Não foi possível salvar a localização do post '{postId}'", _context);
                        // Continue without location - it's optional
                    }
                }

                // Notify analytics service about new post creation
                NotifyPostCreated(postId);

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
                    }),
                    Task.Run(async () =>
                    {
                        try
                        {
                            IPostLocationDao locationDao = scope.ServiceProvider.GetRequiredService<IPostLocationDao>();
                            PostLocation? location = await locationDao.GetPostLocationByPostIdAsync(postId);
                            if (location != null)
                            {
                                await locationDao.DeleteAsync(location);
                            }
                        }
                        catch(Exception){ /* Ignorar erros de deleção de localização */ }
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

        private void NotifyPostCreated(Guid postId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostCreatedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostCreatedService>();

            notifyService.NotifyPostCreated(postId);
        }

    }
}