using GeoCidadao.Caching.Contracts;
using GeoCidadao.Database.Cache;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Cache
{
    public class UserProfileDaoCache(IInMemoryCacheService cacheService) : RepositoryCache<UserProfile>(cacheService), IUserProfileDaoCache
    {

    }
}