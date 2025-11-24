namespace GeoCidadao.FeedMapAPI.Models.DTOs
{
    public class MapPostDTO
    {
        public Guid Id { get; set; }
        public AuthorDTO Author { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public LocationDTO Location { get; set; } = null!;
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public string? FirstMediaUrl { get; set; }
        public double RelevanceScore { get; set; }
    }
}
