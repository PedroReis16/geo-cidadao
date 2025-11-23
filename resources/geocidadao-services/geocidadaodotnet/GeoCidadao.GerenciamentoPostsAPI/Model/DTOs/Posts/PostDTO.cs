using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Position;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public PostPositionDTO? Position { get; set; }

        public PostDTO()
        {

        }

        public PostDTO(Post post)
        {
            Id = post.Id;
            Content = post.Content;
            UserId = post.UserId;
            CreatedAt = post.CreatedAt;
            Position = post.Location != null ? new PostPositionDTO(post.Location) : null;
        }
    }
}