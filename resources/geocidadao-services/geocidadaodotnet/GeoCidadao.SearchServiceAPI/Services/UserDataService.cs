using GeoCidadao.SearchServiceAPI.Models;
using GeoCidadao.SearchServiceAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using GeoCidadao.Database;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.SearchServiceAPI.Services;

/// <summary>
/// Service to fetch user data from Users database
/// </summary>
public class UserDataService : IUserDataService
{
    private readonly GeoDbContext _dbContext;
    private readonly ILogger<UserDataService> _logger;

    public UserDataService(
        GeoDbContext dbContext,
        ILogger<UserDataService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UserDocument?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await _dbContext.Set<UserProfile>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return null;
            }

            return MapToUserDocument(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", userId);
            return null;
        }
    }

    public async Task<IEnumerable<UserDocument>> GetAllUsersAsync()
    {
        try
        {
            var users = await _dbContext.Set<UserProfile>()
                .AsNoTracking()
                .ToListAsync();

            return users.Select(MapToUserDocument).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all users");
            return new List<UserDocument>();
        }
    }

    private UserDocument MapToUserDocument(UserProfile user)
    {
        return new UserDocument
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt ?? user.CreatedAt,
            IsDeleted = false,
            IsActive = true
        };
    }
}
