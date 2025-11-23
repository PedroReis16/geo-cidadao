using GeoCidadao.Caching.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices
{
    public interface IUserCacheService : IInMemoryCacheService
    {
        void AddUserCache(Guid userId, UserDTO user);
        UserDTO? GetUser(Guid userId);
        void RemoveUser(Guid userId);
    }
}