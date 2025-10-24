using Application.Shared.Ports;
using Domain.Customers.Entities;
using Domain.Shared.ValueObjects;

namespace Application.Customers.Ports;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(Email email);
}