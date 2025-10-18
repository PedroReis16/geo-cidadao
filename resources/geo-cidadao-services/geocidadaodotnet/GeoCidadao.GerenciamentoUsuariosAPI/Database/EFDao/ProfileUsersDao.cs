using GeoCidadao.Database;
using GeoCidadao.Database.CacheContracts;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Model.Entities;
using GeoCidadao.Model.Enums;
using GeoCidadao.Model.Exceptions;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao
{
    public class ProfileUsersDao(GeoDbContext context, IRepositoryCache<UserProfile>? cache = null) : BaseDao<UserProfile>(context, cache), IProfileUserDao
    {
        protected override IRepositoryCache<UserProfile>? GetCache() => _cache as IProfileUserDaoCache;

        protected override Task ValidateEntityForInsert(params UserProfile[] obj)
        {
            foreach (UserProfile user in obj)
            {
                if (string.IsNullOrEmpty(user.Username))
                    throw new EntityValidationException(nameof(user.Username), "Necessário informar o username do usuário", ErrorCodes.USERNAME_REQUIRED);
                if (string.IsNullOrEmpty(user.Email))
                    throw new EntityValidationException(nameof(user.Email), "Necessário informar o email do usuário", ErrorCodes.EMAIL_REQUIRED);
                if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
                    throw new EntityValidationException(nameof(user.FirstName), "Necessário informar o nome ou sobrenome do usuário", ErrorCodes.NAME_REQUIRED);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params UserProfile[] obj)
        {
            throw new NotImplementedException();
        }
    }
}