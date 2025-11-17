using GeoCidadao.SearchServiceAPI.Models;
using GeoCidadao.SearchServiceAPI.Models.DTOs;

namespace GeoCidadao.SearchServiceAPI.Services.Contracts;

/// <summary>
/// Service for search operations
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Search for posts
    /// </summary>
    Task<SearchResponseDTO<PostDocument>> SearchPostsAsync(SearchRequestDTO request);
    
    /// <summary>
    /// Search for users
    /// </summary>
    Task<SearchResponseDTO<UserDocument>> SearchUsersAsync(SearchRequestDTO request);
}
