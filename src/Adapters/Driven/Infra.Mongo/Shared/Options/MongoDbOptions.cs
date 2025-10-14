namespace Infra.Mongo.Shared.Options;

public sealed record MongoDbOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
}