namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class NewPostDTO
    {
        public string Content { get; set; } = null!;
        public PositionDTO? Position { get; set; }
    }
}