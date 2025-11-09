using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao
{
    public class UserPictureDao(GeoDbContext context, IUserPictureDaoCache? cache = null) : BaseDao<UserPicture>(context, cache), IUserPictureDao
    {
        protected override IUserPictureDaoCache? GetCache() => _cache as IUserPictureDaoCache;

        protected override Task ValidateEntityForInsert(params UserPicture[] obj)
        {
            foreach (UserPicture picture in obj)
            {
                if (string.IsNullOrEmpty(picture.FileHash))
                    throw new EntityValidationException(nameof(picture.FileHash), "Necessário informar o hash do arquivo", ErrorCodes.FILE_HASH_REQUIRED);
                if (string.IsNullOrEmpty(picture.FileExtension))
                    throw new EntityValidationException(nameof(picture.FileExtension), "Necessário informar a extensão do arquivo", ErrorCodes.FILE_EXTENSION_REQUIRED);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params UserPicture[] obj)
        {
            return ValidateEntityForInsert(obj);
        }


        public async Task AddOrUpdatePictureAsync(Guid userId, string fileExtension, string fileHash)
        {
            DbSet<UserPicture> dbSet = _context.Set<UserPicture>();

            UserPicture? existingPicture = await dbSet.Where(p => p.Id == userId).FirstOrDefaultAsync();


            if (existingPicture != null)
            {
                existingPicture.FileHash = fileHash;
                existingPicture.UpdatedAt = DateTime.Now.ToUniversalTime();

                await ValidateEntityForUpdate(existingPicture);

                dbSet.Update(existingPicture);
                _cache?.RemoveEntity(existingPicture);
            }
            else
            {
                UserProfile? userProfile = await _context.Set<UserProfile>().Where(up => up.Id == userId).FirstOrDefaultAsync();

                if (userProfile == null)
                    throw new EntityValidationException(nameof(UserProfile), "Usuário não encontrado para adicionar foto", ErrorCodes.USER_NOT_FOUND);

                UserPicture newPicture = new UserPicture
                {
                    Id = userId,
                    FileHash = fileHash,
                    FileExtension = fileExtension,
                    User = userProfile,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    UpdatedAt = DateTime.Now.ToUniversalTime()
                };

                await ValidateEntityForInsert(newPicture);

                dbSet.Add(newPicture);
            }

            await _context.SaveChangesAsync();
        }
    }
}