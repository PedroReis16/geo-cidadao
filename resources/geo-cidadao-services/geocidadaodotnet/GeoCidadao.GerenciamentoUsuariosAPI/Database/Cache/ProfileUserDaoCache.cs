using GeoCidadao.Caching.Contracts;
using GeoCidadao.Database.Cache;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.Model.Entities;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Cache
{
    public class ProfileUserDaoCache(IInMemoryCacheService cacheService) : RepositoryCache<UserProfile>(cacheService), IProfileUserDaoCache
    {

    }
}