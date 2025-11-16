using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Model.DTOs
{
    public class TopProblemDTO
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public PostCategory Category { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public double RelevanceScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
