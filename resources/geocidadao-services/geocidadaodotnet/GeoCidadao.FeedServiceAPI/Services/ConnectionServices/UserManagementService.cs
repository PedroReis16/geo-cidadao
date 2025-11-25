using System.Text.Json;
using GeoCidadao.FeedServiceAPI.Contracts.CacheServices;
using GeoCidadao.FeedServiceAPI.Contracts.ConnectionServices;
using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedServiceAPI.Services.ConnectionServices
{
    public class UserManagementService(HttpClient httpClient, IUserInterestsCacheService cacheService) : IUserManagementService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IUserInterestsCacheService _cacheService = cacheService;

        public async Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId)
        {
            UserInterestsDTO? result = _cacheService.GetUserInterests(userId);

            if (result != null)
                return result;

            HttpResponseMessage response = await _httpClient.GetAsync($"user-interests/{userId}");

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            if(string.IsNullOrWhiteSpace(responseContent))
                return null;

            result = JsonSerializer.Deserialize<UserInterestsDTO>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result != null)
                _cacheService.AddUserInterests(userId, result);

            return result;
        }
    }
}