namespace GeoCidadao.Models.Entities.EngagementServiceAPI
{
    public class CommentLike : BaseEntity
    {
        public Guid UserId { get; set; }

        public PostComment Comment { get; set; } = null!;
    }
}