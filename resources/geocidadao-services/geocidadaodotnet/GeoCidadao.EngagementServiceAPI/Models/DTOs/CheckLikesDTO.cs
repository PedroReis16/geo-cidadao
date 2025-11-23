namespace GeoCidadao.EngagementServiceAPI.Models.DTOs
{
    public class CheckLikesDTO
    {
        public Guid UserId { get; set; }
        public List<Guid> PostIds { get; set; } = new();
    }
}
