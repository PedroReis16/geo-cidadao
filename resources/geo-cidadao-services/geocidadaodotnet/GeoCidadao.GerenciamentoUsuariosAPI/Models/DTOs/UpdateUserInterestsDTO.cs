using GeoCidadao.Models.Enums;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs
{
    public class UpdateUserInterestsDTO
    {
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public List<PostCategory>? Categories { get; set; }
    }
}
