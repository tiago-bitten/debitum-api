using MongoDB.Bson.Serialization.Attributes;

namespace Infra.Mongo.Shared.Documents;

public abstract class EntityDeletableDocument : EntityDocument
{
    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }
}