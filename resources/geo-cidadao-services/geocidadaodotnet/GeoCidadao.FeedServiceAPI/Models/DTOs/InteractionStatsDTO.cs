namespace GeoCidadao.FeedServiceAPI.Models.DTOs
{
    /// <summary>
    /// Estatísticas de interações de um post
    /// </summary>
    public class InteractionStatsDTO
    {
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
