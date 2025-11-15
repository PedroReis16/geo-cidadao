using GeoCidadao.Models.Enums;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs
{
    public class UserInterestsDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public List<PostCategory>? Categories { get; set; }
    }
}
