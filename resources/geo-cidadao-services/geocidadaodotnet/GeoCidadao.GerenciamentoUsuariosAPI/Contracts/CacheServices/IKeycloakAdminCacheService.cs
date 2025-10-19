using GeoCidadao.Caching.Contracts;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices
{
    public interface IKeycloakAdminCacheService : IInMemoryCacheService
    {
        void AddToken(string token);
        string? GetToken();
        void RemoveToken();
    }
}