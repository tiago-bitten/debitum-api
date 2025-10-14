// Projeto: Infra.Mongo/Shared/Repositories/MongoRepositoryBase.cs

using Application.Shared.Ports;
using Domain.Shared.Entities;
using Infra.Mongo.Shared.Documents;
using MongoDB.Driver;

namespace Infra.Mongo.Shared.Repositories;

public abstract class MongoRepositoryBase<TDocument, TEntity>(IMongoDatabase database, string collectionName)
    : IRepository<TEntity>
    where TDocument : EntityDocument
    where TEntity : Entity
{
    protected readonly IMongoCollection<TDocument> Collection = database.GetCollection<TDocument>(collectionName);

    protected abstract TEntity ToDomain(TDocument document);
    protected abstract TDocument ToDocument(TEntity entity);

    public virtual async Task AddAsync(TEntity entity)
    {
        var document = ToDocument(entity);
        document.PublicId = $"{entity.GetType().Name.ToLower()}_{Guid.CreateVersion7():N}";
        await Collection.InsertOneAsync(document);
    }

    public virtual async Task<TEntity?> GetByIdAsync(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, id);
        var document = await Collection.Find(filter).FirstOrDefaultAsync();

        return document is null ? null : ToDomain(document);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        var document = ToDocument(entity);
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, entity.Id);
        await Collection.ReplaceOneAsync(filter, document);
    }

    public virtual Task DeleteAsync(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, id);
        return Collection.DeleteOneAsync(filter);
    }
}