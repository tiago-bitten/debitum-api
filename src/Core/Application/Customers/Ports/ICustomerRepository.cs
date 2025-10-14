using Application.Shared.Ports;
using Domain.Customers.Entities;

namespace Application.Customers.Ports;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
}