using GeoCidadao.FeedMapAPI.Contracts.ConnectionServices;

namespace GeoCidadao.FeedMapAPI.Services.ConnectionServices
{
    public class PostEngagementService : IPostEngagementService
    {
        // public async Task<List<Guid>> GetLikedPostIdsAsync(Guid userId, List<Guid> postIds)
        // {
        //     try
        //     {
        //         // Assuming EngagementService has an endpoint to check likes for a list of posts
        //         // POST /interactions/likes/check { userId, postIds }
        //         // Or GET /users/{userId}/likes?postIds=...

        //         // Since I don't have control over EngagementService API right now to add a bulk endpoint,
        //         // I will assume there is one or I would need to add it.
        //         // For this exercise, let's assume we can fetch all liked post IDs for a user (might be large) or check individually.
        //         // Checking individually is slow.
        //         // Let's assume we send a POST request to check multiple.

        //         var response = await _httpClient.PostAsJsonAsync("interactions/check-likes", new { UserId = userId, PostIds = postIds });

        //         if (!response.IsSuccessStatusCode) return new List<Guid>();

        //         return await response.Content.ReadFromJsonAsync<List<Guid>>() ?? new List<Guid>();
        //     }
        //     catch
        //     {
        //         return new List<Guid>();
        //     }
        // }
    }
}