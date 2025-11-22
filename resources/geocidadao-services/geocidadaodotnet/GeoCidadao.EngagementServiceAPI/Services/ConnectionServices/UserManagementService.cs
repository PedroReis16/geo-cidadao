using GeoCidadao.EngagementServiceAPI.Contracts.ConnectionServices;
using GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO;

namespace GeoCidadao.EngagementServiceAPI.Services.ConnectionServices
{
    public class UserManagementService(HttpClient httpClient) : IUserManagementService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<UserDTO?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"profile/{userId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || !response.IsSuccessStatusCode)
                return null;

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            UserDTO? user = System.Text.Json.JsonSerializer.Deserialize<UserDTO>(responseContent, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("Failed to deserialize user profile response.");
            return user;
        }
    }
}