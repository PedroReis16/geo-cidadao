using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Model.Entities
{
    /// <summary>
    /// Aggregated metrics for a specific region
    /// </summary>
    public class RegionMetrics : BaseEntity
    {
        public string RegionIdentifier { get; set; } = string.Empty; // City-State combination or lat-long grid
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        
        // Aggregated statistics
        public int TotalPosts { get; set; }
        public Dictionary<PostCategory, int> PostsByCategory { get; set; } = new();
        public DateTime LastUpdated { get; set; }
        
        // Top problems (categories with most posts)
        public PostCategory? MostFrequentCategory { get; set; }
        public int MostFrequentCategoryCount { get; set; }
    }
}
