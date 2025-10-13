using GeoCidadao.Database;
using GeoCidadao.Database.CacheContracts;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Model.Entities;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao
{
    public class ProfileUsersDao(GeoDbContext context, IRepositoryCache<UserProfile>? cache = null) : BaseDao<UserProfile>(context, cache), IProfileUserDao
    {
        protected override IRepositoryCache<UserProfile>? GetCache()
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateEntityForInsert(params UserProfile[] obj)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateEntityForUpdate(params UserProfile[] obj)
        {
            throw new NotImplementedException();
        }
    }
}