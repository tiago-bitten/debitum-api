using Domain.Customers.Enums;
using Domain.Customers.Errors;
using Domain.Shared.Entities;
using Domain.Shared.Results;
using Domain.Shared.ValueObjects;

namespace Domain.Customers.Entities;

public sealed class Customer : EntityDeletable
{
    private Customer()
    {
    }

    private Customer(string name, Email email, Phone phone, CustomerOrigem origem)
        => (Name, Email, Phone, Origem) = (name, email, phone, origem);

    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Phone Phone { get; private set; } = null!;
    public CustomerOrigem Origem { get; private set; }

    public static Result<Customer> Create(string name, Email email, Phone phone, CustomerOrigem origem)
    {
        if (string.IsNullOrEmpty(name))
            return CustomerErrors.NameRequired;

        return new Customer(name, email, phone, origem);
    }

    public static Customer Load(
        string id,
        string publicId,
        DateTimeOffset createdAt,
        string name,
        Email email,
        Phone phone,
        CustomerOrigem origem)
        => new()
        {
            Id = id,
            PublicId = publicId,
            Name = name,
            Email = email,
            Phone = phone,
            Origem = origem,
            CreatedAt = createdAt
        };
}