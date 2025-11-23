using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Position
{
    public class PostPositionDTO
    {
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }

        public PostPositionDTO(PostLocation postLocation)
        {
            Latitude = postLocation.Location.Y.ToString();
            Longitude = postLocation.Location.X.ToString();
            Address = postLocation.Address;
            City = postLocation.City;
            State = postLocation.State;
            Country = postLocation.Country;
        }
    }
}