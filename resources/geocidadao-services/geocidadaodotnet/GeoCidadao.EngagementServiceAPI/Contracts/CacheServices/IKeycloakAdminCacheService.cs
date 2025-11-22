using GeoCidadao.Caching.Contracts;

namespace GeoCidadao.EngagementServiceAPI.Contracts.CacheServices
{
    public interface IKeycloakAdminCacheService : IInMemoryCacheService
    {
        void AddToken(string token);
        string? GetToken();
        void RemoveToken();
    }
}