using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.FeedServiceAPI.Contracts.CacheServices;
using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.FeedServiceAPI.Services.CacheServices
{
    public class UserInterestsCacheService(InMemoryCacheConfig config, IMemoryCache memoryCache) : InMemoryCacheService(config, memoryCache), IUserInterestsCacheService
    {
        public void AddUserInterests(Guid userId, UserInterestsDTO interests) => base.Add(userId.ToString(), interests);
        public UserInterestsDTO? GetUserInterests(Guid userId) => base.Get(userId.ToString()) as UserInterestsDTO;
        public void RemoveUserInterests(Guid userId) => base.Remove(userId.ToString());
    }
}