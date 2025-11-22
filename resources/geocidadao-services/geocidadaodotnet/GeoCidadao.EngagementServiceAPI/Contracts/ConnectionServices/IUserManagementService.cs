using GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO;

namespace GeoCidadao.EngagementServiceAPI.Contracts.ConnectionServices
{
    public interface IUserManagementService
    {
        Task<UserDTO?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken);
    }
}