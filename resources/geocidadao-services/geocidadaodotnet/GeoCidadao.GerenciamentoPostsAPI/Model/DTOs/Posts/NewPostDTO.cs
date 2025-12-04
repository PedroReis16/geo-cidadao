using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Position;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class NewPostDTO
    {
        public string Content { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<IFormFile> MediaFiles { get; set; } = new();


        public bool HasPosition => Latitude.HasValue && Longitude.HasValue;
    }
}