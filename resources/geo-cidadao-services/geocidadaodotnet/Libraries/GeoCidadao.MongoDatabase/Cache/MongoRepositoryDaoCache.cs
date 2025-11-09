using GeoCidadao.Caching.Contracts;
using GeoCidadao.Models.MongoEntities;
using GeoCidadao.MongoDatabase.CacheContracts;

namespace GeoCidadao.MongoDatabase.Cache
{
    public class MongoRepositoryDaoCache<TEntity>(IInMemoryCacheService cacheService) : IMongoRepositoryDaoCache<TEntity> where TEntity : BaseMongoEntity
    {
        protected IInMemoryCacheService _cacheService = cacheService;
        private static string GetCacheKey(string key) => $"{typeof(TEntity).Name}-{key}";

        public virtual void AddEntity(TEntity entity)
        {
            _cacheService.Add(MongoRepositoryDaoCache<TEntity>.GetCacheKey(entity.Id.ToString()), entity);
        }

        public virtual void AddEntity(string key, TEntity entity)
        {
            _cacheService.Add(MongoRepositoryDaoCache<TEntity>.GetCacheKey(key), entity);
        }

        public void ClearAll()
        {
            _cacheService.Clear();
        }

        public virtual void RemoveEntity(params TEntity[] entities)
        {
            foreach (TEntity entity in entities)
            {
                _cacheService.Remove(MongoRepositoryDaoCache<TEntity>.GetCacheKey(entity.Id.ToString()));
            }
        }

        public virtual void RemoveEntity(string key)
        {
            _cacheService.Remove(MongoRepositoryDaoCache<TEntity>.GetCacheKey(key));
        }

        public virtual TEntity? GetEntity(string key)
        {
            return _cacheService.Get(MongoRepositoryDaoCache<TEntity>.GetCacheKey(key)) as TEntity;
        }
    }
}