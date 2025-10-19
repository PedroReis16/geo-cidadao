namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices
{
    public interface IKeycloakService
    {
        Task GetUserAsync(Guid userId);
    }
}