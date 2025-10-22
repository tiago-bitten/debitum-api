using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Postgres.Shared.Persistence;

public sealed class DebitumDbContextFactory : IDesignTimeDbContextFactory<DebitumDbContext>
{
    public DebitumDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DebitumDbContext>();

        // Use a default connection string for migrations
        // This will be overridden at runtime by the actual configuration
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=DebitumDb;Username=postgres;Password=postgres");

        return new DebitumDbContext(optionsBuilder.Options);
    }
}
