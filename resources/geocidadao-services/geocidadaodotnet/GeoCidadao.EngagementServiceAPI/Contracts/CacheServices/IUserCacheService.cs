using GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO;

namespace GeoCidadao.EngagementServiceAPI.Contracts.CacheServices
{
    public interface IUserCacheService
    {
        void AddUser(UserDTO user);
        UserDTO? GetUser(Guid userId);
        void RemoveUser(Guid userId);
    }
}