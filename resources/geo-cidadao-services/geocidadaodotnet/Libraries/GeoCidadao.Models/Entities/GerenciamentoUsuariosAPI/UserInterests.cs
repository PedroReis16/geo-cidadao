using GeoCidadao.Models.Enums;

namespace GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI
{
    public class UserInterests : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public List<PostCategory>? Categories { get; set; }
    }
}
