using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.MongoEntities;
using GeoCidadao.MongoDatabase.CacheContracts;
using GeoCidadao.MongoDatabase.Contracts;
using GeoCidadao.MongoDatabase.Helpers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GeoCidadao.MongoDatabase.MongoDao
{
    public abstract class BaseMongoDao<TEntity> : IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        protected IMongoCollection<TEntity> Collection;
        protected IMongoRepositoryDaoCache<TEntity>? _cache;


        public BaseMongoDao(IMongoDatabase database, IMongoRepositoryDaoCache<TEntity>? cache = null)
        {
            Collection = database.GetCollection<TEntity>(MongoHelper.GetCollectionName<TEntity>()) ?? throw new ArgumentNullException(nameof(database));
            _cache = cache;

            UpdateCollectionIndexes();
        }
        private void UpdateCollectionIndexes()
        {
            var indexes = CollectionIndexes();

            indexes.Add(
                new CreateIndexModel<TEntity>(
                    Builders<TEntity>.IndexKeys.Ascending(e => e.CreatedAt)
                ));
            indexes.Add(
                new CreateIndexModel<TEntity>(
                    Builders<TEntity>.IndexKeys.Ascending(e => e.UpdatedAt)
                ));

            if (indexes.Count == 0) return;

            var existingIndexes = Collection.Indexes.List().ToList();

            var existingIndexMap = existingIndexes
                .Where(i => i.Contains("name") && i.Contains("key"))
                .ToDictionary(
                    i => i["name"].AsString,
                    i => i["key"].AsBsonDocument
                );

            var newIndexes = new List<CreateIndexModel<TEntity>>();
            var definedIndexNames = new HashSet<string>();

            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes[i].Options == null)
                    indexes[i] = new CreateIndexModel<TEntity>(indexes[i].Keys, new CreateIndexOptions());

                string? indexName = indexes[i].Options.Name;

                if (string.IsNullOrEmpty(indexName))
                {
                    indexName = GenerateIndexName(indexes[i].Keys);
                    indexes[i].Options.Name = indexName; // Define para comparar depois
                }

                definedIndexNames.Add(indexName);

                // Gera a key do índice como o Mongo vê
                var renderedKey = indexes[i].Keys.Render(
                    args: new()
                    {
                        SerializerRegistry = BsonSerializer.SerializerRegistry,
                        DocumentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TEntity>()
                    }
                );

                if (existingIndexMap.TryGetValue(indexName, out var existingKey))
                {
                    if (!renderedKey.Equals(existingKey))
                    {
                        // Existe com mesmo nome, mas com definição diferente
                        Collection.Indexes.DropOne(indexName);
                        newIndexes.Add(indexes[i]); // Recria com a definição correta
                    }
                }
                else
                    newIndexes.Add(indexes[i]);
            }

            if (newIndexes.Any())
                Collection.Indexes.CreateMany(newIndexes);

            // Remove índices que existem no banco, mas não estão mais definidos
            foreach (var existing in existingIndexMap)
            {
                if (existing.Key != "_id_" && !definedIndexNames.Contains(existing.Key))
                {
                    Collection.Indexes.DropOne(existing.Key);
                }
            }
        }

        private string GenerateIndexName(IndexKeysDefinition<TEntity> keys)
        {
            var rendered = keys.Render(
                args: new()
                {
                    SerializerRegistry = BsonSerializer.SerializerRegistry,
                    DocumentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TEntity>()
                }
            );

            return string.Join("_", rendered.Elements.Select(e => $"{e.Name}_{e.Value}"));
        }

        protected abstract Task ValidateForInsert(params TEntity[] obj);
        protected abstract Task ValidateForUpdate(params TEntity[] obj);

        protected abstract IMongoRepositoryDaoCache<TEntity>? GetCache();
        protected abstract List<CreateIndexModel<TEntity>> CollectionIndexes();


        public virtual async Task<List<TEntity>> AllAsync()
        {
            return await Collection.Find(_ => true).ToListAsync();
        }

        public virtual async Task<TEntity?> FindAsync(object key)
        {
            TEntity? result = null;

            if (_cache != null)
            {
                result = _cache.GetEntity(key.ToString()!);
                if (result != null)
                {
                    return result;
                }
            }

            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, key);
            result = await Collection.Find(filter).FirstOrDefaultAsync();

            if (result != null)
                _cache?.AddEntity(result);
            return result;
        }

        public virtual async Task<int> AddAsync(params TEntity[] obj)
        {
            await ValidateForInsert(obj);

            foreach (var item in obj)
            {
                await Collection.InsertOneAsync(item);
            }

            return obj.Length;
        }

        public virtual async Task<int> UpdateAsync(params TEntity[] obj)
        {
            await ValidateForUpdate(obj);
            int updatedCount = 0;

            foreach (var item in obj)
            {
                TEntity? trackedEntity = await FindAsync(item.Id);
                if (trackedEntity == null)
                    throw new EntityValidationException(nameof(TEntity), $"Objeto não encontrado: {item.Id}", ErrorCodes.ENTITY_NOT_FOUND);

                FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, item.Id);
                ReplaceOneResult updateResult = await Collection.ReplaceOneAsync(filter, item);
                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    updatedCount++;
                }
            }

            _cache?.RemoveEntity(obj);
            return updatedCount;
        }

        public virtual async Task<int> DeleteAsync(params TEntity[] obj)
        {
            int result = 0;
            foreach (var item in obj)
            {
                TEntity? trackedEntity = await FindAsync(item.Id);

                if (trackedEntity == null)
                    throw new EntityValidationException(nameof(TEntity), $"Objeto não encontrado: {item.Id}", ErrorCodes.ENTITY_NOT_FOUND);

                FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, item.Id);
                DeleteResult deleteResult = await Collection.DeleteOneAsync(filter);

                if (deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0)
                {
                    result++;
                }
            }
            _cache?.RemoveEntity(obj);
            return result;
        }

        public virtual async Task<int> DeleteAsync(params object[] keys)
        {
            int result = 0;

            foreach (var key in keys)
            {
                TEntity? trackedEntity = await FindAsync(key);
                if (trackedEntity == null)
                    throw new EntityValidationException(nameof(TEntity), $"Objeto não encontrado: {key}", ErrorCodes.ENTITY_NOT_FOUND);

                FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, key);
                DeleteResult deleteResult = await Collection.DeleteOneAsync(filter);

                if (deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0)
                {
                    result++;
                    _cache?.RemoveEntity(key.ToString()!);
                }
            }
            return result;
        }
    }
}