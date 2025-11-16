using GeoCidadao.AnalyticsServiceAPI.Model.Entities;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Database.Contracts
{
    public interface IPostAnalyticsDao
    {
        Task<PostAnalytics?> FindByPostIdAsync(Guid postId);
        Task AddAsync(PostAnalytics postAnalytics);
        Task<List<PostAnalytics>> GetTopPostsByRelevanceAsync(string? city = null, string? state = null, PostCategory? category = null, int limit = 10);
        Task<List<PostAnalytics>> GetPostsByRegionAsync(string? city, string? state, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> CountPostsInRegionAsync(string? city, string? state);
    }
}
