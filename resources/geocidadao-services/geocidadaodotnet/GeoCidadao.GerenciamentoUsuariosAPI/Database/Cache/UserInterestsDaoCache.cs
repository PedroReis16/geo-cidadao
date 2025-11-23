using GeoCidadao.Caching.Contracts;
using GeoCidadao.Database.Cache;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Cache
{
    public class UserInterestsDaoCache(IInMemoryCacheService cacheService) : RepositoryCache<UserInterests>(cacheService), IUserInterestsDaoCache
    {

    }
}