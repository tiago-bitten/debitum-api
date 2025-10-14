using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace Infra.Mongo.Shared.Extensions;

public static class MongoDbConventions
{
    public static void RegisterConventions()
    {
        const string conventionName = "EnumStringConvention";

        try
        {
            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register(conventionName, pack, _ => true);
        }
        catch
        {
            // ignore
        }
    }
}