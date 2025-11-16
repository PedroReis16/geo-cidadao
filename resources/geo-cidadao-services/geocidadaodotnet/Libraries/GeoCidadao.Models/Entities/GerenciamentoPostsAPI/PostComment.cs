namespace GeoCidadao.Models.Entities.GerenciamentoPostsAPI
{
    public class PostComment : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;

        // Navigation properties
        public Post? Post { get; set; }
    }
}
