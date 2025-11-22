using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.EngagementServiceAPI.Contracts.CacheServices;
using GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.EngagementServiceAPI.Services.CacheServices
{
    public class UserCacheService(InMemoryCacheConfig config, IMemoryCache memoryCache) : InMemoryCacheService(config, memoryCache), IUserCacheService
    {
        public void AddUser(UserDTO user) => base.Add(user.Id.ToString(), user);

        public UserDTO? GetUser(Guid userId) => base.Get(userId.ToString()) as UserDTO;

        public void RemoveUser(Guid userId) => base.Remove(userId.ToString());
    }
}