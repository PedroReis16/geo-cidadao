using System.ComponentModel.DataAnnotations;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class CreateCommentDTO
    {
        [Required(ErrorMessage = "O conteúdo do comentário é obrigatório")]
        [MaxLength(500, ErrorMessage = "O conteúdo do comentário não pode exceder 500 caracteres")]
        public string Content { get; set; } = string.Empty;
    }
}
