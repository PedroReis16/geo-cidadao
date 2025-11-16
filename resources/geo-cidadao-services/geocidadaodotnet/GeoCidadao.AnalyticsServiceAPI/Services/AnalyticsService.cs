using GeoCidadao.AnalyticsServiceAPI.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.DTOs;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IPostAnalyticsDao _postAnalyticsDao;
        private readonly IRegionMetricsDao _regionMetricsDao;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            IPostAnalyticsDao postAnalyticsDao,
            IRegionMetricsDao regionMetricsDao,
            ILogger<AnalyticsService> logger)
        {
            _postAnalyticsDao = postAnalyticsDao;
            _regionMetricsDao = regionMetricsDao;
            _logger = logger;
        }

        public async Task<RegionSummaryDTO?> GetRegionSummaryAsync(string regionIdentifier)
        {
            try
            {
                var regionMetrics = await _regionMetricsDao.FindByRegionIdentifierAsync(regionIdentifier);
                
                if (regionMetrics == null)
                    return null;

                return new RegionSummaryDTO
                {
                    RegionIdentifier = regionMetrics.RegionIdentifier,
                    City = regionMetrics.City,
                    State = regionMetrics.State,
                    Country = regionMetrics.Country,
                    TotalPosts = regionMetrics.TotalPosts,
                    PostsByCategory = regionMetrics.PostsByCategory.ToDictionary(
                        kvp => kvp.Key.ToString(), 
                        kvp => kvp.Value
                    ),
                    LastUpdated = regionMetrics.LastUpdated,
                    MostFrequentCategory = regionMetrics.MostFrequentCategory,
                    MostFrequentCategoryCount = regionMetrics.MostFrequentCategoryCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting region summary for {regionIdentifier}");
                throw;
            }
        }

        public async Task<List<TopProblemDTO>> GetTopProblemsAsync(
            string? region = null, 
            string? period = null, 
            PostCategory? category = null,
            int limit = 10)
        {
            try
            {
                // Parse region if provided (format: "City-State")
                string? city = null;
                string? state = null;
                
                if (!string.IsNullOrEmpty(region))
                {
                    var parts = region.Split('-');
                    if (parts.Length >= 1 && parts[0] != "Unknown")
                        city = parts[0];
                    if (parts.Length >= 2 && parts[1] != "Unknown")
                        state = parts[1];
                }

                var posts = await _postAnalyticsDao.GetTopPostsByRelevanceAsync(city, state, category, limit);

                return posts.Select(p => new TopProblemDTO
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    Category = p.Category,
                    City = p.City,
                    State = p.State,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    LikesCount = p.LikesCount,
                    CommentsCount = p.CommentsCount,
                    RelevanceScore = p.RelevanceScore,
                    CreatedAt = p.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top problems");
                throw;
            }
        }

        public async Task<List<HotspotDTO>> GetHotspotsAsync(int limit = 20)
        {
            try
            {
                var topRegions = await _regionMetricsDao.GetTopRegionsByPostCountAsync(limit);
                
                // Calculate max post count for normalization
                var maxPostCount = topRegions.Any() ? topRegions.Max(r => r.TotalPosts) : 1;

                return topRegions.Select(r => new HotspotDTO
                {
                    RegionIdentifier = r.RegionIdentifier,
                    City = r.City,
                    State = r.State,
                    PostCount = r.TotalPosts,
                    HeatScore = maxPostCount > 0 ? (double)r.TotalPosts / maxPostCount : 0
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hotspots");
                throw;
            }
        }
    }
}
