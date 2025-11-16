namespace GeoCidadao.FeedServiceAPI.Models.DTOs
{
    /// <summary>
    /// Informações do autor de um post
    /// </summary>
    public class AuthorDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
