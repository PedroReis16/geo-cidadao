using System.Net;
using System.Text.Json;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.ConnectionServices
{
    internal class KeycloakService(HttpClient httpClient) : IKeycloakService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<UserDTO?> GetUserAsync(Guid userId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw new Exception($"Erro ao obter o usuário do Keycloak. Status Code: {response.StatusCode}");
            }

            string content = await response.Content.ReadAsStringAsync();

            UserDTO? user = JsonSerializer.Deserialize<UserDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return user;
        }

        public Task DeleteUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(Guid userId, UpdateUserDTO updatedProfile)
        {
            throw new NotImplementedException();
        }
    }
}