using Application.Debts.Ports;
using Domain.Debts.Entities;
using Infra.Postgres.Shared.Persistence;
using Infra.Postgres.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infra.Postgres.Debts.Repositories;

internal sealed class DebtRepository(DebitumDbContext context)
    : RepositoryBase<Debt>(context), IDebtRepository
{
    public async Task<IEnumerable<Debt>> GetByDebtorIdAsync(
        Guid debtorId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.DebtorId == debtorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Debt>> GetDebtsNeedingReminderAsync(
        CancellationToken cancellationToken = default)
    {
        var debts = await DbSet
            .Include(d => d.RemindersSent)
            .Where(d => !d.IsPaid)
            .ToListAsync(cancellationToken);

        return debts.Where(d => d.ShouldSendReminder()).ToList();
    }
}
