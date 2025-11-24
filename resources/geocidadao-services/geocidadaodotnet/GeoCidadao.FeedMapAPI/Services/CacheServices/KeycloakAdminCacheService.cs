using GeoCidadao.Caching.Services;
using GeoCidadao.FeedMapAPI.Contracts.CacheServices;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.FeedMapAPI.Services.CacheServices
{
    internal class KeycloakAdminCacheService(IMemoryCache memoryCache) : InMemoryCacheService(new(30), memoryCache), IKeycloakAdminCacheService
    {
        private static string GetCacheKey() => "kc_admin_access_token";
        public void AddToken(string token) => base.Add(GetCacheKey(), token);
        public string? GetToken() => base.Get(GetCacheKey()) as string;
        public void RemoveToken() => base.Remove(GetCacheKey());
    }
}