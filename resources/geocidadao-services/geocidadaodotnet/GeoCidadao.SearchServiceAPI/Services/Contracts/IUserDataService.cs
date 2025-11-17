using GeoCidadao.SearchServiceAPI.Models;

namespace GeoCidadao.SearchServiceAPI.Services.Contracts;

/// <summary>
/// Service to fetch user data from Users API
/// </summary>
public interface IUserDataService
{
    /// <summary>
    /// Get a user by ID from Users API
    /// </summary>
    Task<UserDocument?> GetUserByIdAsync(Guid userId);
    
    /// <summary>
    /// Get all users from Users API (for bulk reindexing)
    /// </summary>
    Task<IEnumerable<UserDocument>> GetAllUsersAsync();
}
