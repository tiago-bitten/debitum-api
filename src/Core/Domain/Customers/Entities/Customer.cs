using Domain.Customers.Enums;
using Domain.Customers.Errors;
using Domain.Customers.Events;
using Domain.Shared.Entities;
using Domain.Shared.Results;
using Domain.Shared.ValueObjects;

namespace Domain.Customers.Entities;

public sealed class Customer : EntityDeletable
{
    private Customer()
    {
    }

    private Customer(string name, Email email, Phone phone, CustomerOrigin origin)
        => (Name, Email, Phone, Origin) = (name, email, phone, origin);

    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Phone Phone { get; private set; } = null!;
    public CustomerOrigin Origin { get; private set; }

    public static Result<Customer> Create(string name, Email email, Phone phone, CustomerOrigin origin)
    {
        if (string.IsNullOrEmpty(name))
            return CustomerErrors.NameRequired;

        var customer = new Customer(name, email, phone, origin);
        customer.Raise(new CustomerCreatedEvent(customer.Id));

        return customer;
    }
}