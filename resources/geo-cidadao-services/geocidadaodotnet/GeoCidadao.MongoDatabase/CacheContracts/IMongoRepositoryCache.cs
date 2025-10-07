using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCidadao.Model.MongoEntities;

namespace GeoCidadao.MongoDatabase.CacheContracts
{
    public interface IMongoRepositoryDaoCache<TEntity> where TEntity : BaseMongoEntity
    {
        void AddEntity(TEntity entity);
        void AddEntity(string key, TEntity entity);
        TEntity? GetEntity(string key);
        void RemoveEntity(string key);
        void RemoveEntity(params TEntity[] entities);
        void ClearAll();
    }
}