using Application.Shared.Ports;
using Domain.Debts.Entities;

namespace Application.Debts.Ports;

public interface IDebtorRepository : IRepository<Debtor>
{
    Task<IEnumerable<Debtor>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}