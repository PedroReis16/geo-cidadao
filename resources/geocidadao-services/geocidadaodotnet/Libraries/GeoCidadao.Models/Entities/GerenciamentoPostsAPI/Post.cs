using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.Models.Entities.GerenciamentoPostsAPI
{
    public class Post : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;

        public PostCategory Category { get; set; }

        public List<PostMedia>? Medias { get; set; } = new();
        public PostLocation? Location { get; set; }
    }
}