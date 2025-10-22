namespace Infra.Postgres.Shared.Options;

public sealed record PostgresOptions
{
    public string ConnectionString { get; init; } = string.Empty;
}
