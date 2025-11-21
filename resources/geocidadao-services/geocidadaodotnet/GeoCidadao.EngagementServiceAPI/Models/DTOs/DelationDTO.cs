using GeoCidadao.Models.Enums;

namespace GeoCidadao.EngagementServiceAPI.Models.DTOs
{
    public class DelationDTO
    {
        public DelationTypes Type { get; set; }
        public string? ReasonDetails { get; set; }
    }
}