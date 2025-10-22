using Application.Customers.Ports;
using Application.Shared.Ports;
using Domain.Customers.Entities;
using Infra.Mongo.Customers.Documents;
using Infra.Mongo.Customers.Mappers;
using Infra.Mongo.Shared.Repositories;
using MongoDB.Driver;

namespace Infra.Mongo.Customers.Repositories;

internal sealed class CustomerRepository(
    IMongoDatabase database,
    IEventsDispatcher eventsDispatcher)
    : MongoRepositoryBase<CustomerDocument, Customer>(database, "Customers", eventsDispatcher), ICustomerRepository
{
    protected override Customer ToDomain(CustomerDocument document) => document.ToDomain();
    protected override CustomerDocument ToDocument(Customer entity) => entity.ToDocument();

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        var filter = Builders<CustomerDocument>.Filter.Eq(d => d.Email, email);
        var document = await Collection.Find(filter).FirstOrDefaultAsync();

        return document is null ? null : ToDomain(document);
    }
}