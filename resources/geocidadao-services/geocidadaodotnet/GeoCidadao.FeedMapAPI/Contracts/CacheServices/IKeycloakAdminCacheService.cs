using GeoCidadao.Caching.Contracts;

namespace GeoCidadao.FeedMapAPI.Contracts.CacheServices
{
    public interface IKeycloakAdminCacheService : IInMemoryCacheService
    {
        void AddToken(string token);
        string? GetToken();
        void RemoveToken();
    }
}