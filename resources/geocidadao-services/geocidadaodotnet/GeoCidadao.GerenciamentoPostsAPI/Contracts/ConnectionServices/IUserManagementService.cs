using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.UserManagement;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.ConnectionServices
{
    public interface IUserManagementService
    {
        Task<UserDTO?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
