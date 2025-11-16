using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class PostLikeDTO
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public PostLikeDTO()
        {
        }

        public PostLikeDTO(PostLike like)
        {
            Id = like.Id;
            PostId = like.PostId;
            UserId = like.UserId;
            CreatedAt = like.CreatedAt;
        }
    }
}
