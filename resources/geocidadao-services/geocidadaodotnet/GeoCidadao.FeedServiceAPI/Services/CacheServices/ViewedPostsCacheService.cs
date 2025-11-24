using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.FeedServiceAPI.Contracts.CacheServices;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.FeedServiceAPI.Services.CacheServices
{
    public class ViewedPostsCacheService(InMemoryCacheConfig config, IMemoryCache memoryCache) : InMemoryCacheService(config, memoryCache), IViewedPostsCacheService
    {

    }
}