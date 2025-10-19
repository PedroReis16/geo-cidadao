using GeoCidadao.Database.Contracts;
using GeoCidadao.Model.Entities;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts
{
    public interface IUserProfileDao : IRepository<UserProfile>
    {
        Task UpdateUserPictureAsync(Guid userId, string objectKey, string fileHash);
    }
}