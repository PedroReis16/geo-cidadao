namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class NewPostDTO
    {
        public string Text { get; set; } = null!;
        public List<IFormFile>? Media { get; set; }
        public PositionDTO? Position { get; set; }
    }
}