using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts
{
    public interface IUserProfileDao : IRepository<UserProfile>
    {
        Task UpdateUserPictureAsync(Guid userId, string objectKey, string fileHash);
    }
}