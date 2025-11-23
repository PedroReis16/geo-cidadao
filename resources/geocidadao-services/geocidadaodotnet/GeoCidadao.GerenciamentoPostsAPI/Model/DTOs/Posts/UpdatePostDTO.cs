using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Position;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class UpdatePostDTO
    {
        public string? Content { get; set; }
        public NewPostPositionDTO? Position { get; set; }
    }
}