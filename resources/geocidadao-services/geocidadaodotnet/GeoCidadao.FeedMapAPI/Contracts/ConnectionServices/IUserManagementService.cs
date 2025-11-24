using GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedMapAPI.Contracts.ConnectionServices
{
    public interface IUserManagementService
    {
        Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId);
    }
}