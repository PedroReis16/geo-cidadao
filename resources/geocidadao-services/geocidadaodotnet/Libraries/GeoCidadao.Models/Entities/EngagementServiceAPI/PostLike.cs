namespace GeoCidadao.Models.Entities.EngagementServiceAPI
{
    public class PostLike : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}