using Application.Customers.Ports;
using Domain.Customers.Entities;
using Domain.Shared.ValueObjects;
using Infra.Postgres.Shared.Persistence;
using Infra.Postgres.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infra.Postgres.Customers.Repositories;

internal sealed class CustomerRepository(DebitumDbContext context)
    : RepositoryBase<Customer>(context), ICustomerRepository
{
    public Task<Customer?> GetByEmailAsync(Email email)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Email == email.Address);
    }
}