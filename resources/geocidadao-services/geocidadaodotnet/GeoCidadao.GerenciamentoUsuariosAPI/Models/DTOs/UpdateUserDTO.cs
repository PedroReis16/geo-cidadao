namespace GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    }
}