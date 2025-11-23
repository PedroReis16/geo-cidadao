using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostLocationDao(GeoDbContext context, IPostLocationDaoCache? cache = null) : BaseDao<PostLocation>(context, cache), IPostLocationDao
    {
        protected override IPostLocationDaoCache? GetCache() => _cache as IPostLocationDaoCache;

        protected override Task ValidateEntityForInsert(params PostLocation[] obj)
        {
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostLocation[] obj)
        {
            return ValidateEntityForInsert(obj);
        }

    }
}