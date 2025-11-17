using GeoCidadao.Models.Enums;

namespace GeoCidadao.AMQP.Messages
{
    /// <summary>
    /// Message containing enriched post data for analytics processing
    /// </summary>
    public class PostAnalyticsMessage
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
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public double RelevanceScore { get; set; }
    }
}
