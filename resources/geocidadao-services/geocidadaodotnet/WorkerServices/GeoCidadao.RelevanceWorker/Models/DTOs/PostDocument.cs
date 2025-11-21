using System.Text.Json.Serialization;

namespace GeoCidadao.RelevanceWorker.Models.DTOs
{
    public class PostDocument
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        
        public double RelevanceScore { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}