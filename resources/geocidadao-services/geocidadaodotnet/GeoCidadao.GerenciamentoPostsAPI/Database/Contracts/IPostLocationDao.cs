using GeoCidadao.Database.Contracts;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using NetTopologySuite.Geometries;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Contracts
{
    public interface IPostLocationDao : IRepository<PostLocation>
    {
        Task<List<PostLocation>> GetPostsWithinRadiusAsync(Point center, double radiusKm, int? itemsCount = null, int? pageNumber = null);
        Task<List<PostLocation>> GetPostsByLocationAsync(string? city = null, string? state = null, string? country = null, int? itemsCount = null, int? pageNumber = null);
        Task<PostLocation?> GetPostLocationByPostIdAsync(Guid postId);
    }
}