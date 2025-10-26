using GeoCidadao.Caching.Contracts;
using GeoCidadao.Database.Cache;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.Model.Entities;
using GeoCidadao.Model.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.Cache
{
    public class UserPictureDaoCache(IInMemoryCacheService cacheService) : RepositoryCache<UserPicture>(cacheService), IUserPictureDaoCache
    {

    }
}