using System.Text.Json.Serialization;

namespace GeoCidadao.PostIndexerWorker.Models.DTOs
{
    public class PostDocument
    {
        public Guid PostOwnerId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>(); //Categorias da postagem
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Enriched Data
        public string? AuthorName { get; set; }
        public string? AuthorUsername { get; set; }
        public string? AuthorProfilePicture { get; set; }
        public List<string> MediaUrls { get; set; } = new();

        // Relevance Data (Initialized to 0)
        public double RelevanceScore { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}