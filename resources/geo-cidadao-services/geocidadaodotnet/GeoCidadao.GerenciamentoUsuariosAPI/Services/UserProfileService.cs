using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services
{
    internal class UserProfileService(IKeycloakService keycloakService) : IUserProfileService
    {
        private readonly IKeycloakService _keycloakService = keycloakService;

        public async Task GetUserProfileAsync(Guid userId)
        {
            await _keycloakService.GetUserAsync(userId);
        }
    }
}