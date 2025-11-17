namespace GeoCidadao.FeedServiceAPI.Models.External
{
    /// <summary>
    /// Usuário DTO retornado pela API de Usuários
    /// </summary>
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
