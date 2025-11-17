using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts
{
    public class PostWithLocationDTO : PostDTO
    {
        public PostLocationDTO? Location { get; set; }

        public PostWithLocationDTO() : base()
        {
        }

        public PostWithLocationDTO(Post post, PostLocationDTO? location = null) : base(post)
        {
            Location = location;
        }
    }
}
