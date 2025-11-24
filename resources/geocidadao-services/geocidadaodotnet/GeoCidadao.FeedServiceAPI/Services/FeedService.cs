using System.Collections.Concurrent;
using Elastic.Clients.Elasticsearch;
using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Contracts.CacheServices;
using GeoCidadao.FeedServiceAPI.Contracts.ConnectionServices;
using GeoCidadao.FeedServiceAPI.Database.Documents;
using GeoCidadao.FeedServiceAPI.Models.DTOs;
using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedServiceAPI.Services
{
    public class FeedService(ILogger<FeedService> logger, IServiceScopeFactory scopeFactory, IHttpContextAccessor? httpContextAccessor) : IFeedService
    {
        private readonly ILogger<FeedService> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        
        public async Task<List<PostDTO>> GetFeedAsync(Guid userId, int page, int pageSize)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                IUserManagementService userManagementService = scope.ServiceProvider.GetRequiredService<IUserManagementService>();
                IViewedPostsCacheService viewedPostsService = scope.ServiceProvider.GetRequiredService<IViewedPostsCacheService>();
                IPostsDaoService postsDaoService = scope.ServiceProvider.GetRequiredService<IPostsDaoService>();

                UserInterestsDTO interests = await userManagementService.GetUserInterestsAsync(userId) ?? new();

                List<Guid> viewedPosts = viewedPostsService.GetViewedPosts(userId);

                List<PostDocument> posts = await postsDaoService.GetPostsAsync(interests, viewedPosts, page, pageSize);

                ConcurrentBag<PostDTO> results = new();

                Parallel.ForEach(posts, post =>
                {
                    PostDTO result = new()
                    {
                        Id = post.Id,
                        Content = post.Content,
                        CreatedAt = post.CreatedAt,
                        Author = new AuthorDTO
                        {
                            Id = post.PostOwnerId,
                            Name = post.AuthorName!,
                            Username = post.AuthorUsername!,
                            ProfilePictureUrl = post.AuthorProfilePicture
                        },
                        Media = post.MediaUrls,
                        LikesCount = post.LikesCount,
                        CommentsCount = post.CommentsCount,
                    };

                    if (post.Latitude.HasValue && post.Longitude.HasValue)
                    {
                        result.Location = new LocationDTO
                        {
                            Latitude = post.Latitude.Value,
                            Longitude = post.Longitude.Value,
                            City = post.City ?? string.Empty
                        };
                    }

                    results.Add(result);
                });

                _ = Task.Run(() => viewedPostsService.UpdateViewedPosts(userId, posts.Select(p => p.Id).ToList()));

                return results.ToList();
            }
            catch (Exception ex)
            {
                string errorMessage = "Ocorreu um erro ao obter os dados para o feed do usuário";

                _logger.LogError(ex, errorMessage);
                throw;
            }
        }


    }
}
