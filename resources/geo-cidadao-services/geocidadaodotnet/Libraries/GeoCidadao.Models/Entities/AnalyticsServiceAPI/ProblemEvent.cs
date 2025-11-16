using GeoCidadao.Models.Enums;

namespace GeoCidadao.Models.Entities.AnalyticsServiceAPI
{
    /// <summary>
    /// Represents an individual problem event logged for analytics
    /// </summary>
    public class ProblemEvent : BaseEntity
    {
        public Guid PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PostCategory Category { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime EventTimestamp { get; set; }
        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public double RelevanceScore { get; set; } = 0.0;
    }
}
