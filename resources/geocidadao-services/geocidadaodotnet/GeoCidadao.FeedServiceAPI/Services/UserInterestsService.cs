using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Model;
using GeoCidadao.FeedServiceAPI.Config;
using Microsoft.Extensions.Options;

namespace GeoCidadao.FeedServiceAPI.Services
{
    public class UserInterestsService(HttpClient httpClient) : IUserInterestsService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"UserInterests/{userId}");
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<UserInterestsDTO>();
            }
            catch
            {
                return null;
            }
        }
    }
}
