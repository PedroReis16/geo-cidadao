
namespace GeoCidadao.FeedServiceAPI.Contracts.CacheServices
{
    public interface IViewedPostsCacheService
    {
        List<Guid> GetViewedPosts(Guid userId);
        void UpdateViewedPosts(Guid userId, List<Guid> postIds);
    }
}