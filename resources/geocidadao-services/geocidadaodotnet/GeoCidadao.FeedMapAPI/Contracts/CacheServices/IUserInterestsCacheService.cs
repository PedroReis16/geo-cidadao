using GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedMapAPI.Contracts.CacheServices
{
    public interface IUserInterestsCacheService
    {
        void AddUserInterests(Guid userId, UserInterestsDTO interests);
        UserInterestsDTO? GetUserInterests(Guid userId);
        void RemoveUserInterests(Guid userId);
    }
}