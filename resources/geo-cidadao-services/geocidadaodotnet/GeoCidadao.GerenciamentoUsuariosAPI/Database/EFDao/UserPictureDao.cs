using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Model.Entities;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao
{
    public class UserPictureDao(GeoDbContext context, IUserPictureDaoCache? cache = null) : BaseDao<UserPicture>(context, cache), IUserPictureDao
    {
        protected override IUserPictureDaoCache? GetCache() => _cache as IUserPictureDaoCache;

        protected override Task ValidateEntityForInsert(params UserPicture[] obj)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateEntityForUpdate(params UserPicture[] obj)
        {
            throw new NotImplementedException();
        }
    }
}