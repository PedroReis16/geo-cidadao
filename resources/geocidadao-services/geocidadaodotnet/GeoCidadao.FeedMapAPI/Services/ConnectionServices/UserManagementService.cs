using System.Text.Json;
using GeoCidadao.FeedMapAPI.Contracts.CacheServices;
using GeoCidadao.FeedMapAPI.Contracts.ConnectionServices;
using GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedMapAPI.Services.ConnectionServices
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