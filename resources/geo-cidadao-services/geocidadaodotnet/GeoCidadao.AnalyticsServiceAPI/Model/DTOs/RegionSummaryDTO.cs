using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Model.DTOs
{
    public class RegionSummaryDTO
    {
        public string RegionIdentifier { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public int TotalPosts { get; set; }
        public Dictionary<string, int> PostsByCategory { get; set; } = new();
        public DateTime LastUpdated { get; set; }
        public PostCategory? MostFrequentCategory { get; set; }
        public int MostFrequentCategoryCount { get; set; }
    }
}
