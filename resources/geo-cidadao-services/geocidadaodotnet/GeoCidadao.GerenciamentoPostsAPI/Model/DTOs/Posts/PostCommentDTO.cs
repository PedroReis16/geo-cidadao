using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class PostCommentDTO
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public PostCommentDTO()
        {
        }

        public PostCommentDTO(PostComment comment)
        {
            Id = comment.Id;
            PostId = comment.PostId;
            UserId = comment.UserId;
            Content = comment.Content;
            CreatedAt = comment.CreatedAt;
            UpdatedAt = comment.UpdatedAt;
        }
    }
}
