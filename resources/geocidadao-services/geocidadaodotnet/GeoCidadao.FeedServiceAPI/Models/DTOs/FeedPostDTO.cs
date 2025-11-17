namespace GeoCidadao.FeedServiceAPI.Models.DTOs
{
    /// <summary>
    /// Post completo para exibição no feed
    /// </summary>
    public class FeedPostDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public AuthorDTO Author { get; set; } = new();
        public InteractionStatsDTO Interactions { get; set; } = new();
    }
}
