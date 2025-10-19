using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices
{
    public interface IKeycloakService
    {
        Task DeleteUserAsync(Guid userId);
        Task<UserDTO?> GetUserAsync(Guid userId);
        Task UpdateUserAsync(Guid userId, UpdateUserDTO updatedProfile);
    }
}