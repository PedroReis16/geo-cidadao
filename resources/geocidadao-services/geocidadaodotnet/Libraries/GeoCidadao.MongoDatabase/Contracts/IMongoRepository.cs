using GeoCidadao.Models.MongoEntities;

namespace GeoCidadao.MongoDatabase.Contracts
{
    public interface IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        /// <summary>
        /// Get all entities async
        /// </summary>
        /// <returns>Entities list</returns>
        Task<List<TEntity>> AllAsync();

        /// <summary>
        /// Get an entity async by its primary key
        /// </summary>
        /// <param name="key">Primary key</param>
        /// <returns>Entity</returns>
        Task<TEntity?> FindAsync(object key);

        /// <summary>
        /// Insert one or many entities async
        /// </summary>
        /// <param name="obj">Entities</param>
        /// <returns>Entities affected count</returns>
        Task<int> AddAsync(params TEntity[] obj);

        /// <summary>
        /// Update one or many entities async
        /// </summary>
        /// <param name="obj">Entities</param>
        /// <returns>Entities affected count</returns>
        Task<int> UpdateAsync(params TEntity[] obj);

        /// <summary>
        /// Delete one or many entities async
        /// </summary>
        /// <param name="obj">Entities</param>
        /// <returns>Entities deleted count</returns>
        Task<int> DeleteAsync(params TEntity[] obj);

        /// <summary>
        /// Delete one or many entities async
        /// </summary>
        /// <param name="obj">Entities keys</param>
        /// <returns>Entities deleted count</returns>
        Task<int> DeleteAsync(params object[] keys);
    }
}