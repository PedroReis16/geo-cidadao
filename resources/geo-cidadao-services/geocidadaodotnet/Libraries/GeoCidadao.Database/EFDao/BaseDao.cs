using GeoCidadao.Database.CacheContracts;
using GeoCidadao.Database.Contracts;
using GeoCidadao.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.Database.EFDao
{
    public abstract class BaseDao<TEntity>(GeoDbContext context, IRepositoryCache<TEntity>? cache = null) : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected GeoDbContext _context = context;

        protected IRepositoryCache<TEntity>? _cache = cache;

        protected abstract Task ValidateEntityForInsert(params TEntity[] obj);

        protected abstract Task ValidateEntityForUpdate(params TEntity[] obj);

        protected abstract IRepositoryCache<TEntity>? GetCache();

        public virtual async Task<int> AddAsync(params TEntity[] obj)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            await ValidateEntityForInsert(obj);
            dbSet.AddRange(obj);
            int result = await _context.SaveChangesAsync();
            return result;
        }

        public virtual async Task<int> UpdateAsync(params TEntity[] obj)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            foreach (TEntity entity in obj)
            {
                TEntity? existingEntity = await dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id.Equals(entity.Id));

                if (existingEntity == null)
                    continue;

                await ValidateEntityForUpdate(entity);
                entity.UpdatedAt = DateTime.Now.ToUniversalTime();
                _ = dbSet.Update(entity);
            }

            int result = await _context.SaveChangesAsync();

            if (result > 0)
                _cache?.RemoveEntity(obj);

            return result;
        }

        public virtual async Task<List<TEntity>> AllAsync(bool track = false)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            List<TEntity> list = track ?
                await dbSet.ToListAsync() :
                await dbSet.AsNoTracking().ToListAsync();
            return list;
        }

        public virtual async Task<int> DeleteAsync(params TEntity[] obj)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            dbSet.RemoveRange(obj);
            int result = await _context.SaveChangesAsync();
            if (result > 0)
                _cache?.RemoveEntity(obj);

            return result;
        }

        public virtual async Task<int> DeleteAsync(params object[] keys)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            foreach (object item in keys)
            {
                TEntity? entity = await dbSet.FindAsync(item);
                if (entity != null)
                {
                    entity.UpdatedAt = DateTime.Now.ToUniversalTime();
                    _ = dbSet.Update(entity);
                    _cache?.RemoveEntity(entity);
                }
            }
            int result = await _context.SaveChangesAsync();
            return result;
        }

        public virtual async Task<TEntity?> FindAsync(object key, bool track = false)
        {
            TEntity? entity = null;

            if (key != null && _cache != null && !track)
            {
                entity = _cache.GetEntity(key.ToString()!);
                if (entity != null)
                    return entity;
            }

            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            entity = track ?
                await dbSet.FindAsync(key) :
                await dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(key));

            if (entity != null)
            {
                if (!track)
                    _cache?.AddEntity(entity);
                return entity;
            }

            return null;
        }

        public async Task<int> RestoreAsync(params TEntity[] obj)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            foreach (TEntity item in obj)
            {
                item.UpdatedAt = DateTime.Now.ToUniversalTime();
            }
            dbSet.UpdateRange(obj);
            int result = await _context.SaveChangesAsync();

            if (result > 0)
                _cache?.RemoveEntity(obj);

            return result;
        }
    }
}