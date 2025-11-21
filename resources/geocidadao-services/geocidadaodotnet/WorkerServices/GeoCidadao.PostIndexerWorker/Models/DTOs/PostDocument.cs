namespace GeoCidadao.PostIndexerWorker.Models.DTOs
{
    public class PostDocument
    {
        public Guid Id { get; set; }
        public Guid PostOwnerId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>(); //Categorias da postagem
        public double RelevanceScore { get; set; } = 1.0;
    }
}