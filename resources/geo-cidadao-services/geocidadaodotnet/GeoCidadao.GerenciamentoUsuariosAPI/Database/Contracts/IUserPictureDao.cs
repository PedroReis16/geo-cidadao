using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts
{
    public interface IUserPictureDao : IRepository<UserPicture>
    {
        Task AddOrUpdatePictureAsync(Guid userId, string objectKey, string fileHash);
    }
}