using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedServiceAPI.Contracts.ConnectionServices
{
    public interface IUserManagementService
    {
        Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId);
    }
}