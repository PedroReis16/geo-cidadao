using GeoCidadao.GerenciamentoPostsAPI.Contracts.ConnectionServices;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.UserManagement;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.ConnectionServices
{
    public class UserManagementService(HttpClient httpClient) : IUserManagementService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<UserDTO?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var profileTask = _httpClient.GetAsync($"profile/{userId}", cancellationToken);
                var pictureTask = _httpClient.GetAsync($"ProfilePicture/{userId}", cancellationToken);

                await Task.WhenAll(profileTask, pictureTask);

                var profileResponse = await profileTask;
                var pictureResponse = await pictureTask;

                if (!profileResponse.IsSuccessStatusCode)
                    return null;

                string profileContent = await profileResponse.Content.ReadAsStringAsync(cancellationToken);

                UserDTO? user = System.Text.Json.JsonSerializer.Deserialize<UserDTO>(profileContent, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user != null && pictureResponse.IsSuccessStatusCode && pictureResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    string pictureUrl = await pictureResponse.Content.ReadAsStringAsync(cancellationToken);
                    if (pictureUrl.StartsWith("\"") && pictureUrl.EndsWith("\""))
                        pictureUrl = pictureUrl.Trim('"');

                    user.PictureUrl = pictureUrl;
                }

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}
