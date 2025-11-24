using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace GeoCidadao.FeedServiceAPI.Models.DTOs
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public List<string>? Media { get; set; }
        public AuthorDTO? Author { get; set; }
        public string Content { get; set; } = string.Empty;
        public LocationDTO? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public string Timestamp { get => CreatedAt.ToString("o"); }
    }
}
