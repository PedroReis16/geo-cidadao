using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostLocationDao(GeoDbContext context, IPostLocationDaoCache? cache = null) : BaseDao<PostLocation>(context, cache), IPostLocationDao
    {
        protected override IPostLocationDaoCache? GetCache() => _cache as IPostLocationDaoCache;

        protected override Task ValidateEntityForInsert(params PostLocation[] obj)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateEntityForUpdate(params PostLocation[] obj)
        {
            throw new NotImplementedException();
        }
    }
}