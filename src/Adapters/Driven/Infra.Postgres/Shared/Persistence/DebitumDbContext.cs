using Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Postgres.Shared.Persistence;

public sealed class DebitumDbContext(DbContextOptions<DebitumDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DebitumDbContext).Assembly);
    }
}