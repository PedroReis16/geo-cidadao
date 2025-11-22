namespace GeoCidadao.Models.Entities.EngagementServiceAPI
{
    public class PostComment : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;

        public List<CommentLike> Likes { get; set; } = null!;
    }
}