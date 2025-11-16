using Microsoft.AspNetCore.Mvc;
using GeoCidadao.SearchServiceAPI.Models.DTOs;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using GeoCidadao.SearchServiceAPI.Models;

namespace GeoCidadao.SearchServiceAPI.Controllers;

/// <summary>
/// Controller for search operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly IElasticsearchIndexService _indexService;
    private readonly IPostDataService _postDataService;
    private readonly IUserDataService _userDataService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        ISearchService searchService,
        IElasticsearchIndexService indexService,
        IPostDataService postDataService,
        IUserDataService userDataService,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _indexService = indexService;
        _postDataService = postDataService;
        _userDataService = userDataService;
        _logger = logger;
    }

    /// <summary>
    /// Search for posts and/or users
    /// </summary>
    /// <param name="request">Search parameters</param>
    /// <returns>Search results</returns>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchRequestDTO request)
    {
        try
        {
            if (request.SearchType.ToLower() == "users")
            {
                var result = await _searchService.SearchUsersAsync(request);
                return Ok(result);
            }
            else if (request.SearchType.ToLower() == "posts")
            {
                var result = await _searchService.SearchPostsAsync(request);
                return Ok(result);
            }
            else
            {
                // Default to posts search
                var result = await _searchService.SearchPostsAsync(request);
                return Ok(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search");
            return StatusCode(500, new { message = "Internal server error during search" });
        }
    }

    /// <summary>
    /// Index a post manually (admin endpoint)
    /// </summary>
    /// <param name="postId">Post ID to index</param>
    [HttpPost("index/post/{postId}")]
    public async Task<IActionResult> IndexPost(Guid postId)
    {
        try
        {
            var postDocument = await _postDataService.GetPostByIdAsync(postId);
            
            if (postDocument == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            var result = await _indexService.IndexPostAsync(postDocument);
            
            if (result)
            {
                return Ok(new { message = "Post indexed successfully" });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to index post" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing post {PostId}", postId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update a post in the index
    /// </summary>
    /// <param name="postId">Post ID to update</param>
    [HttpPut("index/post/{postId}")]
    public async Task<IActionResult> UpdatePost(Guid postId)
    {
        try
        {
            var postDocument = await _postDataService.GetPostByIdAsync(postId);
            
            if (postDocument == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            var result = await _indexService.UpdatePostAsync(postId, postDocument);
            
            if (result)
            {
                return Ok(new { message = "Post updated successfully" });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to update post" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", postId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete a post from the index
    /// </summary>
    /// <param name="postId">Post ID to delete</param>
    [HttpDelete("index/post/{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        try
        {
            var result = await _indexService.DeletePostAsync(postId);
            
            if (result)
            {
                return Ok(new { message = "Post deleted from index successfully" });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to delete post from index" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", postId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Index a user manually (admin endpoint)
    /// </summary>
    /// <param name="userId">User ID to index</param>
    [HttpPost("index/user/{userId}")]
    public async Task<IActionResult> IndexUser(Guid userId)
    {
        try
        {
            var userDocument = await _userDataService.GetUserByIdAsync(userId);
            
            if (userDocument == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await _indexService.IndexUserAsync(userDocument);
            
            if (result)
            {
                return Ok(new { message = "User indexed successfully" });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to index user" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing user {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Trigger a full reindex of all posts (admin endpoint)
    /// </summary>
    [HttpPost("reindex/posts")]
    public async Task<IActionResult> ReindexPosts()
    {
        try
        {
            _logger.LogInformation("Starting full reindex of posts");
            
            var posts = await _postDataService.GetAllPostsAsync();
            var result = await _indexService.BulkIndexPostsAsync(posts);
            
            if (result)
            {
                _logger.LogInformation("Full reindex of posts completed successfully");
                return Ok(new { message = "Posts reindexed successfully", count = posts.Count() });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to reindex posts" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full reindex of posts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Trigger a full reindex of all users (admin endpoint)
    /// </summary>
    [HttpPost("reindex/users")]
    public async Task<IActionResult> ReindexUsers()
    {
        try
        {
            _logger.LogInformation("Starting full reindex of users");
            
            var users = await _userDataService.GetAllUsersAsync();
            var result = await _indexService.BulkIndexUsersAsync(users);
            
            if (result)
            {
                _logger.LogInformation("Full reindex of users completed successfully");
                return Ok(new { message = "Users reindexed successfully", count = users.Count() });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to reindex users" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full reindex of users");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
