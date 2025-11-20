using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IUserInterestsService
    {
        Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId);
        Task UpdateUserFollowedUsersAsync(Guid userId, Guid followedUserId);
        Task UpdateUserFollowedCitiesAsync(Guid userId, string city);
        Task UpdateUserFollowedDistrictsAsync(Guid userId, string district);
        Task UpdateUserFollowedCategoriesAsync(Guid userId, List<PostCategory> categories);
    }
}
