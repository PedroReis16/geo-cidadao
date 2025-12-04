namespace GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement
{
    public class UserInterestsDTO
    {
        public List<Guid> Users { get; set; } = new();
        public List<string> Cities { get; set; } = new();
    }
}
