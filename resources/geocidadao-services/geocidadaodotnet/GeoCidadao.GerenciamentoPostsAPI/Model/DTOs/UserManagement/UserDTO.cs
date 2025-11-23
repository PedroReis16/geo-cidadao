namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.UserManagement
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
    }
}
