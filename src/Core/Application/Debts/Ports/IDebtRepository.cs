using Application.Shared.Ports;
using Domain.Debts.Entities;

namespace Application.Debts.Ports;

public interface IDebtRepository : IRepository<Debt>
{
    Task<IEnumerable<Debt>> GetByDebtorIdAsync(Guid debtorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Debt>> GetDebtsNeedingReminderAsync(CancellationToken cancellationToken = default);
}