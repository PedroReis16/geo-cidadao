using GeoCidadao.Database.Contracts;
using GeoCidadao.Model.Entities;
using GeoCidadao.Model.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts
{
    public interface IUserPictureDao : IRepository<UserPicture>
    {
        Task AddOrUpdatePictureAsync(Guid userId, string objectKey, string fileHash);
    }
}