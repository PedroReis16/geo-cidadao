using GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO;

namespace GeoCidadao.EngagementServiceAPI.Models.DTOs.Comments
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public UserDTO Author { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public int LikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}