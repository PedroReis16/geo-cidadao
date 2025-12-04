using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedServiceAPI.Contracts.CacheServices
{
    public interface IUserInterestsCacheService
    {
        void AddUserInterests(Guid userId, UserInterestsDTO interests);
        UserInterestsDTO? GetUserInterests(Guid userId);
        void RemoveUserInterests(Guid userId);
    }
}