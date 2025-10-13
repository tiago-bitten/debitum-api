using Domain.Customers.Errors;
using Domain.Shared.Entities;
using Domain.Shared.Results;
using Domain.Shared.ValueObjects;

namespace Domain.Customers.Entities;

public sealed class Customer : EntityDeletable
{
    private Customer() {}
    
    private Customer(string name, Email email, Phone phone) 
        => (Name, Email, Phone) = (name, email, phone);
    
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Phone Phone { get; private set; } = null!;

    public static Result<Customer> Create(string name, Email email, Phone phone)
    {
        if (string.IsNullOrEmpty(name))
            return CustomerErrors.NameRequired;

        return new Customer(name, email, phone);
    }
}