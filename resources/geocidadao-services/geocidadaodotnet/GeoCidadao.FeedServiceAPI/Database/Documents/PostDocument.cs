using System.Text.Json.Serialization;

namespace GeoCidadao.FeedServiceAPI.Database.Documents
{
    public class PostDocument
    {
        [JsonPropertyName("_id")]
        public Guid Id { get; set; } // ES Id is usually separate, but we can map it
        public Guid PostOwnerId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public DateTime CreatedAt { get; set; }

        // Enriched Data
        public string? AuthorName { get; set; }
        public string? AuthorUsername { get; set; }
        public string? AuthorProfilePicture { get; set; }
        public List<string> MediaUrls { get; set; } = new();

        // Relevance Data
        public double RelevanceScore { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }

        // Dynamic Data (Not in ES)
        public bool LikedByCurrentUser { get; set; }
    }
}