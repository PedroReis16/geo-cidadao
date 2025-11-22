namespace GeoCidadao.RelevanceWorker.Models.DTOs
{
    public class RelevanceDocument
    {
        public double RelevanceScore { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}