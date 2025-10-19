
namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IUserProfileService
    {
        Task GetUserProfileAsync(Guid userId);
    }
}