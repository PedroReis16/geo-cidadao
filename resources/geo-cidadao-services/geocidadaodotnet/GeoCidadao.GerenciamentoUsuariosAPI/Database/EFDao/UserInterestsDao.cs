using GeoCidadao.Database;
using GeoCidadao.Database.CacheContracts;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao
{
    public class UserInterestsDao : BaseDao<UserInterests>, IUserInterestsDao
    {
        public UserInterestsDao(GeoDbContext context) : base(context, null)
        {
        }

        protected override IRepositoryCache<UserInterests>? GetCache() => null;

        protected override Task ValidateEntityForInsert(params UserInterests[] obj)
        {
            foreach (UserInterests interests in obj)
            {
                if (interests.UserId == Guid.Empty)
                    throw new EntityValidationException(nameof(interests.UserId), "Necessário informar o ID do usuário", ErrorCodes.USER_ID_REQUIRED);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params UserInterests[] obj)
        {
            return Task.CompletedTask;
        }

        public async Task<UserInterests?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<UserInterests>()
                .FirstOrDefaultAsync(ui => ui.UserId == userId);
        }
    }
}
