using System.Net;
using System.Text.Json;
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

        public async Task UpdateUserAsync(Guid userId, UpdateUserDTO updatedProfile)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"users/{userId}", updatedProfile);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"users/{userId}");

            response.EnsureSuccessStatusCode();
        }


    }
}