namespace GeoCidadao.FeedServiceAPI.Models.External
{
    /// <summary>
    /// Post DTO retornado pela API de Posts
    /// </summary>
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
