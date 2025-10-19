using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.CacheServices
{
    public class UserCacheService(InMemoryCacheConfig config, IMemoryCache memoryCache) : InMemoryCacheService(config, memoryCache), IUserCacheService
    {
        public void AddUserCache(Guid userId, UserDTO user) => base.Add(userId.ToString(), user);

        public UserDTO? GetUser(Guid userId) => base.Get(userId.ToString()) as UserDTO;

        public void RemoveUser(Guid userId) => base.Remove(userId.ToString());
    }
}