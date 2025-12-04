using GeoCidadao.FeedMapAPI.Database.Documents;
using GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedMapAPI.Contracts
{
    public interface IPostsDaoService
    {
        Task<List<PostDocument>> GetPostsAsync(UserInterestsDTO interests, List<Guid> viewedPosts, int page, int pageSize);
        
        /// <summary>
        /// Busca postagens dentro de um bounding box geográfico com filtro de relevância
        /// </summary>
        Task<List<PostDocument>> GetPostsInBoundsAsync(
            double topLeftLat,
            double topLeftLon,
            double bottomRightLat,
            double bottomRightLon,
            double minRelevanceScore,
            int limit);
    }
}