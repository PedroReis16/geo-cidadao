using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IUserInterestsService
    {
        Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId);
        Task<UserInterestsDTO> CreateUserInterestsAsync(Guid userId, UpdateUserInterestsDTO interestsDTO);
        Task<UserInterestsDTO> UpdateUserInterestsAsync(Guid userId, UpdateUserInterestsDTO interestsDTO);
        Task DeleteUserInterestsAsync(Guid userId);
    }
}
