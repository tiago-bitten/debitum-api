using Domain.Customers.Entities;
using Domain.Shared.ValueObjects;
using Infra.Mongo.Customers.Documents;
using MongoDB.Bson;

namespace Infra.Mongo.Customers.Mappers;

public static class CustomerMapper
{
    public static Customer ToDomain(this CustomerDocument document)
    {
        var emailResult = Email.Create(document.Email);
        var phoneResult = Phone.Create(document.Phone);
        
        if (emailResult.IsFailure || phoneResult.IsFailure)
            throw new InvalidOperationException($"Fail trying to create entity {document.PublicId}.");
        
        return Customer.Load(
            document.Id.ToString(),
            document.PublicId,
            document.CreatedAt,
            document.Name,
            emailResult.Value,
            phoneResult.Value);
    }
    
    public static CustomerDocument ToDocument(this Customer customer)
    {
        return new CustomerDocument
        {
            Id = ObjectId.Parse(customer.Id),
            PublicId = customer.PublicId,
            Name = customer.Name,
            Email = customer.Email.Address,
            Phone = customer.Phone.Number,
            CreatedAt = customer.CreatedAt,
            IsDeleted = customer.IsDeleted
        };
    }
}