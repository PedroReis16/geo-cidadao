namespace GeoCidadao.EngagementServiceAPI.Models.DTOs.UserManagementDTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
    }
}