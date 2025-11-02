using Application.Debts.Ports;
using Domain.Debts.Entities;
using Infra.Postgres.Shared.Persistence;
using Infra.Postgres.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infra.Postgres.Debts.Repositories;

internal sealed class DebtorRepository(DebitumDbContext context)
    : RepositoryBase<Debtor>(context), IDebtorRepository
{
    public async Task<IEnumerable<Debtor>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }
}
