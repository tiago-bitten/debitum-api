using Application.Customers.Ports;
using Domain.Customers.Entities;
using Infra.Mongo.Customers.Documents;
using Infra.Mongo.Customers.Mappers;
using Infra.Mongo.Shared.Repositories;
using MongoDB.Driver;

namespace Infra.Mongo.Customers.Repositories;

public class CustomerRepository(IMongoDatabase database)
    : MongoRepositoryBase<CustomerDocument, Customer>(database, "Customers"), ICustomerRepository
{
    protected override Customer ToDomain(CustomerDocument document)
    {
        return document.ToDomain();
    }

    protected override CustomerDocument ToDocument(Customer entity)
    {
        return entity.ToDocument();
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        var filter = Builders<CustomerDocument>.Filter.Eq(d => d.Email, email);
        var document = await Collection.Find(filter).FirstOrDefaultAsync();
        
        return document is null ? null : ToDomain(document);
    }
}