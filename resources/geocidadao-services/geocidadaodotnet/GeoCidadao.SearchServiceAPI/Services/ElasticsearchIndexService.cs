using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using GeoCidadao.SearchServiceAPI.Config;
using GeoCidadao.SearchServiceAPI.Models;
using GeoCidadao.SearchServiceAPI.Services.Contracts;

namespace GeoCidadao.SearchServiceAPI.Services;

/// <summary>
/// Implementation of Elasticsearch indexing operations
/// </summary>
public class ElasticsearchIndexService : IElasticsearchIndexService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchIndexService> _logger;
    private readonly string _postsIndex;
    private readonly string _usersIndex;

    public ElasticsearchIndexService(
        ElasticsearchClient client,
        IConfiguration configuration,
        ILogger<ElasticsearchIndexService> logger)
    {
        _client = client;
        _logger = logger;
        
        var esConfig = configuration.GetSection("Elasticsearch").Get<ElasticsearchConfiguration>();
        var defaultIndex = esConfig?.DefaultIndex ?? "geocidadao";
        _postsIndex = $"{defaultIndex}-posts";
        _usersIndex = $"{defaultIndex}-users";
    }

    public async Task InitializeIndicesAsync()
    {
        try
        {
            await CreatePostsIndexAsync();
            await CreateUsersIndexAsync();
            _logger.LogInformation("Elasticsearch indices initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Elasticsearch indices");
            throw;
        }
    }

    private async Task CreatePostsIndexAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(_postsIndex);
        if (existsResponse.Exists)
        {
            _logger.LogInformation("Posts index already exists");
            return;
        }

        var createResponse = await _client.Indices.CreateAsync(_postsIndex, c => c
            .Mappings(m => m
                .Properties<PostDocument>(p => p
                    .Keyword(k => k.Id)
                    .Keyword(k => k.AuthorId)
                    .Text(t => t.AuthorName)
                    .Text(t => t.Content)
                    .Keyword(k => k.Category)
                    .Keyword(k => k.Tags)
                    .Text(t => t.LocationCity)
                    .Text(t => t.LocationNeighborhood)
                    .DoubleNumber(n => n.LocationLatitude)
                    .DoubleNumber(n => n.LocationLongitude)
                    .IntegerNumber(n => n.LikesCount)
                    .IntegerNumber(n => n.CommentsCount)
                    .DoubleNumber(n => n.RelevanceScore)
                    .Date(d => d.CreatedAt)
                    .Date(d => d.UpdatedAt)
                    .Boolean(b => b.IsDeleted)
                    .Boolean(b => b.IsPublic)
                )
            )
        );

        if (!createResponse.IsValidResponse)
        {
            _logger.LogError("Failed to create posts index: {Error}", createResponse.DebugInformation);
            throw new Exception($"Failed to create posts index: {createResponse.DebugInformation}");
        }

        _logger.LogInformation("Posts index created successfully");
    }

    private async Task CreateUsersIndexAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(_usersIndex);
        if (existsResponse.Exists)
        {
            _logger.LogInformation("Users index already exists");
            return;
        }

        var createResponse = await _client.Indices.CreateAsync(_usersIndex, c => c
            .Mappings(m => m
                .Properties<UserDocument>(p => p
                    .Keyword(k => k.Id)
                    .Text(t => t.Username, td => td.Fields(f => f.Keyword("keyword")))
                    .Keyword(k => k.Email)
                    .Text(t => t.FirstName)
                    .Text(t => t.LastName)
                    .Text(t => t.FullName, td => td.Analyzer("portuguese"))
                    .Date(d => d.CreatedAt)
                    .Date(d => d.UpdatedAt)
                    .Boolean(b => b.IsDeleted)
                    .Boolean(b => b.IsActive)
                )
            )
        );

        if (!createResponse.IsValidResponse)
        {
            _logger.LogError("Failed to create users index: {Error}", createResponse.DebugInformation);
            throw new Exception($"Failed to create users index: {createResponse.DebugInformation}");
        }

        _logger.LogInformation("Users index created successfully");
    }

    public async Task<bool> IndexPostAsync(PostDocument post)
    {
        try
        {
            var response = await _client.IndexAsync(post, idx => idx.Index(_postsIndex).Id(post.Id));
            
            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to index post {PostId}: {Error}", post.Id, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Post {PostId} indexed successfully", post.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing post {PostId}", post.Id);
            return false;
        }
    }

    public async Task<bool> UpdatePostAsync(Guid postId, PostDocument post)
    {
        try
        {
            var response = await _client.UpdateAsync<PostDocument, PostDocument>(
                _postsIndex,
                postId,
                u => u.Doc(post).DocAsUpsert(true)
            );

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to update post {PostId}: {Error}", postId, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Post {PostId} updated successfully", postId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", postId);
            return false;
        }
    }

    public async Task<bool> DeletePostAsync(Guid postId)
    {
        try
        {
            var response = await _client.DeleteAsync(_postsIndex, postId);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to delete post {PostId}: {Error}", postId, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Post {PostId} deleted successfully", postId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", postId);
            return false;
        }
    }

    public async Task<bool> IndexUserAsync(UserDocument user)
    {
        try
        {
            var response = await _client.IndexAsync(user, idx => idx.Index(_usersIndex).Id(user.Id));
            
            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to index user {UserId}: {Error}", user.Id, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("User {UserId} indexed successfully", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(Guid userId, UserDocument user)
    {
        try
        {
            var response = await _client.UpdateAsync<UserDocument, UserDocument>(
                _usersIndex,
                userId,
                u => u.Doc(user).DocAsUpsert(true)
            );

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to update user {UserId}: {Error}", userId, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("User {UserId} updated successfully", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        try
        {
            var response = await _client.DeleteAsync(_usersIndex, userId);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to delete user {UserId}: {Error}", userId, response.DebugInformation);
                return false;
            }

            _logger.LogInformation("User {UserId} deleted successfully", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> BulkIndexPostsAsync(IEnumerable<PostDocument> posts)
    {
        try
        {
            var response = await _client.BulkAsync(b => b
                .Index(_postsIndex)
                .IndexMany(posts, (descriptor, post) => descriptor.Id(post.Id))
            );

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to bulk index posts: {Error}", response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Bulk indexed {Count} posts successfully", posts.Count());
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing posts");
            return false;
        }
    }

    public async Task<bool> BulkIndexUsersAsync(IEnumerable<UserDocument> users)
    {
        try
        {
            var response = await _client.BulkAsync(b => b
                .Index(_usersIndex)
                .IndexMany(users, (descriptor, user) => descriptor.Id(user.Id))
            );

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to bulk index users: {Error}", response.DebugInformation);
                return false;
            }

            _logger.LogInformation("Bulk indexed {Count} users successfully", users.Count());
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing users");
            return false;
        }
    }
}
