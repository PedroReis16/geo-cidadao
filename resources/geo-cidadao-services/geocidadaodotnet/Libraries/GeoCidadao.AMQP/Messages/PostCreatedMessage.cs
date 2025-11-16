using GeoCidadao.Models.Enums;

namespace GeoCidadao.AMQP.Messages
{
    public class PostCreatedMessage
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public PostCategory Category { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Location information
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        
        // Engagement metrics
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public double RelevanceScore { get; set; }
    }
}
