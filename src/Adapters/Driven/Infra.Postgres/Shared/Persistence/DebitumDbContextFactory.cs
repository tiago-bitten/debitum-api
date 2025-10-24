using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Postgres.Shared.Persistence;

internal sealed class DebitumDbContextFactory : IDesignTimeDbContextFactory<DebitumDbContext>
{
    public DebitumDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DebitumDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=DebitumDb;Username=postgres;Password=postgres");

        return new DebitumDbContext(optionsBuilder.Options);
    }
}