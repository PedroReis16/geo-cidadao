using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostMediaDao(GeoDbContext context, IPostMediaDaoCache? cache = null) : BaseDao<PostMedia>(context, cache), IPostMediaDao
    {
        protected override IPostMediaDaoCache? GetCache() => _cache as IPostMediaDaoCache;

        protected override Task ValidateEntityForInsert(params PostMedia[] obj)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateEntityForUpdate(params PostMedia[] obj)
        {
            throw new NotImplementedException();
        }
    }
}