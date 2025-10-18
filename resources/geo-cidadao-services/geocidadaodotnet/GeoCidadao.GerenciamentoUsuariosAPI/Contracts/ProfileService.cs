
namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IProfileService
    {
        Task GetUserProfileAsync(Guid userId);
    }
}