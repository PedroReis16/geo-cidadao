using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.FeedServiceAPI.Contracts.CacheServices;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.FeedServiceAPI.Services.CacheServices
{
    public class ViewedPostsCacheService(InMemoryCacheConfig config, IMemoryCache memoryCache) : InMemoryCacheService(config, memoryCache), IViewedPostsCacheService
    {

        public List<Guid> GetViewedPosts(Guid userId) => base.Get(userId.ToString()) as List<Guid> ?? new();

        public void UpdateViewedPosts(Guid userId, List<Guid> postIds)
        {
            Parallel.ForEach(postIds, postId =>
            {
                base.Add(userId.ToString(), postId);
            });
        }
    }
}