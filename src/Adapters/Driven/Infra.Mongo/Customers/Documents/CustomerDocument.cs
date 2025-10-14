using Infra.Mongo.Shared.Documents;
using MongoDB.Bson.Serialization.Attributes;

namespace Infra.Mongo.Customers.Documents;

public sealed class CustomerDocument : EntityDeletableDocument
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    
    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;
}