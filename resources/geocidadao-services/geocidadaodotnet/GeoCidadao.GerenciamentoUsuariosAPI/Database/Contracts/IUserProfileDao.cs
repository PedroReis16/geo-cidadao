using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts
{
    public interface IUserProfileDao : IRepository<UserProfile>
    {
    }
}