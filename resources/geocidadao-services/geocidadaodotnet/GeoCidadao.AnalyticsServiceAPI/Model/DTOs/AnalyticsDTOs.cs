using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Model.DTOs
{
    public class ProblemEventDTO
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PostCategory Category { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime EventTimestamp { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public double RelevanceScore { get; set; }
    }

    public class RegionSummaryDTO
    {
        public string? City { get; set; }
        public string? State { get; set; }
        public int TotalProblems { get; set; }
        public Dictionary<PostCategory, int> ProblemsByCategory { get; set; } = new();
        public List<ProblemEventDTO> MostRelevantProblems { get; set; } = new();
    }

    public class CategoryCountDTO
    {
        public PostCategory Category { get; set; }
        public int Count { get; set; }
    }

    public class HotspotDTO
    {
        public string City { get; set; } = string.Empty;
        public int ProblemCount { get; set; }
    }
}
