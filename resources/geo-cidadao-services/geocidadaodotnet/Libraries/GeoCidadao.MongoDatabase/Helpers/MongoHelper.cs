using GeoCidadao.Models.MongoEntities;

namespace GeoCidadao.MongoDatabase.Helpers
{
    public static class MongoHelper
    {
        public static string GetCollectionName<TEntity>() where TEntity : BaseMongoEntity
        {
            var entityName = typeof(TEntity).Name;

            if (entityName.EndsWith("MongoEntity"))
                entityName = entityName.Substring(0, entityName.Length - "MongoEntity".Length);

            var collectionName = char.ToLowerInvariant(entityName[0]) + entityName.Substring(1);

            if (collectionName.EndsWith("y"))
                collectionName = collectionName.Substring(0, collectionName.Length - 1) + "ies";
            else if (collectionName.EndsWith("s") || collectionName.EndsWith("x") ||
                     collectionName.EndsWith("z") || collectionName.EndsWith("ch") ||
                     collectionName.EndsWith("sh"))
                collectionName += "es";
            else
                collectionName += "s";

            return collectionName;
        }

    }
}