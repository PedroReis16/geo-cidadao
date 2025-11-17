namespace GeoCidadao.Models.Entities.GerenciamentoPostsAPI
{
    public class PostLike : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public Post? Post { get; set; }
    }
}
