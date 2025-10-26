namespace GeoCidadao.Model.Entities.GerenciamentoPostsAPI
{
    public class PostMedia : BaseEntity
    {
        public string MediaType { get; set; } = string.Empty;
        public int Order { get; set; }
        public double FileSize { get; set; } // Size in bytes

        public Post Post { get; set; } = null!;
    }
}