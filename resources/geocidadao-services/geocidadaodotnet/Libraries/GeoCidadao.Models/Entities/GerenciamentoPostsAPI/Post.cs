using GeoCidadao.Models.Enums;

namespace GeoCidadao.Models.Entities.GerenciamentoPostsAPI
{
    public class Post : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;

        public PostCategory Category { get; set; }

        public List<PostMedia>? Medias { get; set; } = new();

        // Interaction metrics
        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public double RelevanceScore { get; set; } = 0.0;

        // Navigation properties
        public List<PostLike>? Likes { get; set; } = new();
        public List<PostComment>? Comments { get; set; } = new();
    }
}