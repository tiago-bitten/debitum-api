using Domain.Customers.Entities;
using Domain.Debts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Postgres.Shared.Persistence;

public sealed class DebitumDbContext(DbContextOptions<DebitumDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Debtor> Debtors => Set<Debtor>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<ReminderSent> RemindersSent => Set<ReminderSent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DebitumDbContext).Assembly);
    }
}