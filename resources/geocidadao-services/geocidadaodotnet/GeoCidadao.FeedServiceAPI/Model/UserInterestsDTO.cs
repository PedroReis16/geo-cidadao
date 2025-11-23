namespace GeoCidadao.FeedServiceAPI.Model
{
    public class UserInterestsDTO
    {
        public List<string> Cities { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}
