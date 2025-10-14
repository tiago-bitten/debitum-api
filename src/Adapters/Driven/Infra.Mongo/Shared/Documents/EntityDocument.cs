using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infra.Mongo.Shared.Documents;

public abstract class EntityDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.Empty;
    
    [BsonElement("public_id")]
    public string PublicId { get; set; } = string.Empty;
    
    [BsonElement("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}