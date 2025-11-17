using GeoCidadao.SearchServiceAPI.Config;
using GeoCidadao.SearchServiceAPI.Models;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using System.Text.Json;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using GeoCidadao.Database;

namespace GeoCidadao.SearchServiceAPI.Services;

/// <summary>
/// Service to fetch post data from Posts database
/// </summary>
public class PostDataService : IPostDataService
{
    private readonly GeoDbContext _dbContext;
    private readonly ILogger<PostDataService> _logger;

    public PostDataService(
        GeoDbContext dbContext,
        ILogger<PostDataService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PostDocument?> GetPostByIdAsync(Guid postId)
    {
        try
        {
            var post = await _dbContext.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                _logger.LogWarning("Post {PostId} not found", postId);
                return null;
            }

            var location = await _dbContext.Set<PostLocation>()
                .AsNoTracking()
                .FirstOrDefaultAsync(pl => pl.PostId == postId);

            return MapToPostDocument(post, location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching post {PostId}", postId);
            return null;
        }
    }

    public async Task<IEnumerable<PostDocument>> GetAllPostsAsync()
    {
        try
        {
            var posts = await _dbContext.Posts
                .AsNoTracking()
                .ToListAsync();

            var postIds = posts.Select(p => p.Id).ToList();
            var locations = await _dbContext.Set<PostLocation>()
                .AsNoTracking()
                .Where(pl => postIds.Contains(pl.PostId))
                .ToDictionaryAsync(pl => pl.PostId);

            return posts.Select(post => MapToPostDocument(
                post,
                locations.ContainsKey(post.Id) ? locations[post.Id] : null
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all posts");
            return new List<PostDocument>();
        }
    }

    private PostDocument MapToPostDocument(Post post, PostLocation? location)
    {
        return new PostDocument
        {
            Id = post.Id,
            AuthorId = post.UserId,
            AuthorName = string.Empty, // Will be populated by subscriber
            Content = post.Content,
            Category = post.Category,
            Tags = new List<string>(), // Can be extended if tags are added to Post model
            LocationCity = null, // Will need to be geocoded or stored separately
            LocationNeighborhood = null,
            LocationLatitude = location?.Position.Y,
            LocationLongitude = location?.Position.X,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            RelevanceScore = post.RelevanceScore,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt ?? post.CreatedAt,
            IsDeleted = false, // Assuming posts in DB are not deleted
            IsPublic = true
        };
    }
}
