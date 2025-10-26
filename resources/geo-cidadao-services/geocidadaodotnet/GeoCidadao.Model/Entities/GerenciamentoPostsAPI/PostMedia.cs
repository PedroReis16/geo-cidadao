namespace GeoCidadao.Model.Entities.GerenciamentoPostsAPI
{
    public class PostMedia : BaseEntity
    {
        public string MediaType { get; set; } = string.Empty;
        public int Order { get; set; }

        public Post Post { get; set; } = null!;
    }
}