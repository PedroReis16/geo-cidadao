using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IUserProfileService
    {
        Task DeleteUserProfileAsync(Guid userId);
        Task<UserDTO?> GetUserProfileAsync(Guid userId);
        Task UpdateUserProfileAsync(Guid userId, UpdateUserDTO updatedProfile);
    }
}