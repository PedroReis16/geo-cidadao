using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeoCidadao.Models.MongoEntities
{
    public abstract class BaseMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("createdAt"), BsonRepresentation(BsonType.DateTime), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt"), BsonRepresentation(BsonType.DateTime), BsonDateTimeOptions(Kind = DateTimeKind.Utc), BsonIgnoreIfNull]
        public DateTime? UpdatedAt { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}