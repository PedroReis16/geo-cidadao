using GeoCidadao.SearchServiceAPI.Models;

namespace GeoCidadao.SearchServiceAPI.Services.Contracts;

/// <summary>
/// Service to fetch post data from Posts API
/// </summary>
public interface IPostDataService
{
    /// <summary>
    /// Get a post by ID from Posts API
    /// </summary>
    Task<PostDocument?> GetPostByIdAsync(Guid postId);
    
    /// <summary>
    /// Get all posts from Posts API (for bulk reindexing)
    /// </summary>
    Task<IEnumerable<PostDocument>> GetAllPostsAsync();
}
