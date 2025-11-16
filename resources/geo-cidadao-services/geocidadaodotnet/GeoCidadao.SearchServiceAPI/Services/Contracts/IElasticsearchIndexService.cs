using GeoCidadao.SearchServiceAPI.Models;

namespace GeoCidadao.SearchServiceAPI.Services.Contracts;

/// <summary>
/// Service for Elasticsearch indexing operations
/// </summary>
public interface IElasticsearchIndexService
{
    /// <summary>
    /// Initialize Elasticsearch indices
    /// </summary>
    Task InitializeIndicesAsync();
    
    /// <summary>
    /// Index a post document
    /// </summary>
    Task<bool> IndexPostAsync(PostDocument post);
    
    /// <summary>
    /// Update a post document
    /// </summary>
    Task<bool> UpdatePostAsync(Guid postId, PostDocument post);
    
    /// <summary>
    /// Delete a post document
    /// </summary>
    Task<bool> DeletePostAsync(Guid postId);
    
    /// <summary>
    /// Index a user document
    /// </summary>
    Task<bool> IndexUserAsync(UserDocument user);
    
    /// <summary>
    /// Update a user document
    /// </summary>
    Task<bool> UpdateUserAsync(Guid userId, UserDocument user);
    
    /// <summary>
    /// Delete a user document
    /// </summary>
    Task<bool> DeleteUserAsync(Guid userId);
    
    /// <summary>
    /// Bulk index posts
    /// </summary>
    Task<bool> BulkIndexPostsAsync(IEnumerable<PostDocument> posts);
    
    /// <summary>
    /// Bulk index users
    /// </summary>
    Task<bool> BulkIndexUsersAsync(IEnumerable<UserDocument> users);
}
