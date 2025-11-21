namespace GeoCidadao.AMQP.Messages
{
    public class NewPostMessage
    {
        public Guid Id { get; set; }
        public Guid PostOwnerId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();//Categorias da postagem
    }
}