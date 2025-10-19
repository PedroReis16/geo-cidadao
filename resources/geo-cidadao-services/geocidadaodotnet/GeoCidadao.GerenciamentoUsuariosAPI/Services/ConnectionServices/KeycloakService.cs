using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.ConnectionServices
{
    internal class KeycloakService(HttpClient httpClient) : IKeycloakService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task GetUserAsync(Guid userId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"users/{userId}");

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
        }
    }
}