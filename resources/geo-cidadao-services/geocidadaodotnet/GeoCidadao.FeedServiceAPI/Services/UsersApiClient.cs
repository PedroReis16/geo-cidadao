using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Models.External;

namespace GeoCidadao.FeedServiceAPI.Services
{
    /// <summary>
    /// Cliente HTTP para comunicação com a API de Usuários
    /// </summary>
    public class UsersApiClient : IUsersApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersApiClient> _logger;

        public UsersApiClient(HttpClient httpClient, ILogger<UsersApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDTO?> GetUserProfileAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"profile/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return user;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return null;
                }

                _logger.LogWarning("Failed to fetch user {UserId}. Status: {StatusCode}", 
                    userId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", userId);
                return null;
            }
        }

        public async Task<Dictionary<Guid, UserDTO>> GetUserProfilesAsync(List<Guid> userIds)
        {
            var result = new Dictionary<Guid, UserDTO>();

            // Busca usuários em paralelo para melhor performance
            var tasks = userIds.Distinct().Select(async userId =>
            {
                var user = await GetUserProfileAsync(userId);
                return new { UserId = userId, User = user };
            });

            var users = await Task.WhenAll(tasks);

            foreach (var item in users)
            {
                if (item.User != null)
                {
                    result[item.UserId] = item.User;
                }
            }

            return result;
        }
    }
}
