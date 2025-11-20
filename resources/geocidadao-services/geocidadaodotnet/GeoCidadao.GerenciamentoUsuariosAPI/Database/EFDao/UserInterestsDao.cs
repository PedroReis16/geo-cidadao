using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao
{
    internal class UserInterestsDao(GeoDbContext context, IUserInterestsDaoCache? cache = null) : BaseDao<UserInterests>(context, cache), IUserInterestsDao
    {
        protected override IUserInterestsDaoCache? GetCache() => _cache as IUserInterestsDaoCache;

        protected override Task ValidateEntityForInsert(params UserInterests[] obj)
        {
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params UserInterests[] obj)
        {
            return Task.CompletedTask;
        }

        public override async Task<UserInterests?> FindAsync(object key, bool track = false)
        {
            UserInterests? result = null;

            if (!track && _cache != null)
            {
                result = _cache.GetEntity(key.ToString()!);
                if (result != null)
                    return result;
            }
            IQueryable<UserInterests> query = _context.Set<UserInterests>().Include(x => x.User).Where(ui => ui.User.Id == (Guid)key);

            if (!track)
                query = query.AsNoTracking();

            result = await query.FirstOrDefaultAsync();

            if (result != null && !track)
                _cache?.AddEntity(result);

            return result;
        }

        public Task UpdateFollowedCategoriesAsync(Guid userId, List<PostCategory> categories)
        {
            DbSet<UserInterests> dbSet = _context.Set<UserInterests>();

            UserInterests? interests = dbSet.Include(x => x.User).Where(ui => ui.User.Id == userId).FirstOrDefault();

            if (interests == null)
                throw new EntityValidationException(nameof(UserInterests), $"O usuário com Id '{userId}' não foi encontrado ou não possui as preferências de postagem configuradas", ErrorCodes.USER_NOT_FOUND);

            foreach (PostCategory category in categories)
            {
                if (interests.FollowedCategories.Contains(category))
                    interests.FollowedCategories.Remove(category);
                else
                    interests.FollowedCategories.Add(category);
            }

            if (interests.FollowedCategories.Count == 0)
                throw new EntityValidationException(nameof(UserInterests), $"O usuário com Id '{userId}' deve seguir ao menos uma categoria de postagem", ErrorCodes.INVALID_USER_INTERESTS);

            interests.UpdatedAt = DateTime.Now.ToUniversalTime();

            dbSet.Update(interests);
            _cache?.RemoveEntity(interests);

            return _context.SaveChangesAsync();
        }

        public Task UpdateFollowedCitiesAsync(Guid userId, string city)
        {
            DbSet<UserInterests> dbSet = _context.Set<UserInterests>();

            UserInterests? interests = dbSet.Include(x => x.User).Where(ui => ui.User.Id == userId).FirstOrDefault();

            if (interests == null)
                throw new EntityValidationException(nameof(UserInterests), $"O usuário com Id '{userId}' não foi encontrado ou não possui as preferências de postagem configuradas", ErrorCodes.USER_NOT_FOUND);

            if (string.IsNullOrEmpty(city))
                return Task.CompletedTask;

            string updatedCity = city.ToLower();

            if (interests.FollowedCities.Contains(updatedCity))
                interests.FollowedCities.Remove(updatedCity);
            else
                interests.FollowedCities.Add(updatedCity);

            interests.UpdatedAt = DateTime.Now.ToUniversalTime();

            dbSet.Update(interests);
            _cache?.RemoveEntity(interests);

            return _context.SaveChangesAsync();
        }

        public Task UpdateFollowedDistrictsAsync(Guid userId, string district)
        {
            DbSet<UserInterests> dbSet = _context.Set<UserInterests>();

            UserInterests? interests = dbSet.Include(x => x.User).Where(ui => ui.User.Id == userId).FirstOrDefault();

            if (string.IsNullOrEmpty(district))
                return Task.CompletedTask;

            if (interests == null)
                throw new EntityValidationException(nameof(UserInterests), $"O usuário com Id '{userId}' não foi encontrado ou não possui as preferências de postagem configuradas", ErrorCodes.USER_NOT_FOUND);

            string updatedDistrict = district.ToLower();

            if (interests.FollowedDistricts.Contains(updatedDistrict))
                interests.FollowedDistricts.Remove(updatedDistrict);
            else
                interests.FollowedDistricts.Add(updatedDistrict);

            interests.UpdatedAt = DateTime.Now.ToUniversalTime();

            dbSet.Update(interests);
            _cache?.RemoveEntity(interests);

            return _context.SaveChangesAsync();
        }

        public Task UpdateFollowedUsersAsync(Guid userId, Guid followedUserId)
        {
            DbSet<UserInterests> dbSet = _context.Set<UserInterests>();

            UserInterests? interests = dbSet.Include(x => x.User).Where(ui => ui.User.Id == userId).FirstOrDefault();

            if (interests == null)
                throw new EntityValidationException(nameof(UserInterests), $"O usuário com Id '{userId}' não foi encontrado ou não possui as preferências de postagem configuradas", ErrorCodes.USER_NOT_FOUND);

            if (interests.FollowedUsers.Contains(followedUserId))
                interests.FollowedUsers.Remove(followedUserId);
            else
                interests.FollowedUsers.Add(followedUserId);

            interests.UpdatedAt = DateTime.Now.ToUniversalTime();

            dbSet.Update(interests);
            _cache?.RemoveEntity(interests);

            return _context.SaveChangesAsync();
        }

    }
}
