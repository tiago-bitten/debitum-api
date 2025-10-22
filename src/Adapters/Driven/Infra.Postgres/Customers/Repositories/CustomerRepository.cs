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
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return null;
        }

        var customer = await DbSet
            .FirstOrDefaultAsync(c => c.Email == emailResult.Value);

        return customer;
    }
}
