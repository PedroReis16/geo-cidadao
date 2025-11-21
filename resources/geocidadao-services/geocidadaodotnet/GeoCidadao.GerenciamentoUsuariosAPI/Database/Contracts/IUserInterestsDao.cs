using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts
{
    public interface IUserInterestsDao : IRepository<UserInterests>
    {
        Task UpdateFollowedCategoriesAsync(Guid userId, List<PostCategory> categories);
        Task UpdateFollowedCitiesAsync(Guid userId, string city);
        Task UpdateFollowedDistrictsAsync(Guid userId, string district);
        Task UpdateFollowedUsersAsync(Guid userId, Guid followedUserId);
    }
}
