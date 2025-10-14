// Projeto: Infra.Mongo/Shared/Repositories/MongoRepositoryBase.cs

using Application.Shared.Ports;
using Domain.Shared.Entities;
using Infra.Mongo.Shared.Documents;
using Infra.Mongo.Shared.Events;
using MongoDB.Driver;

namespace Infra.Mongo.Shared.Repositories;

public abstract class MongoRepositoryBase<TDocument, TEntity>(
    IMongoDatabase database,
    string collectionName,
    IEventsDispatcher eventsDispatcher)
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
        await Collection.InsertOneAsync(document);

        await PublishEventsAsync(entity);
    }

    public virtual async Task<TEntity?> GetByIdAsync(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, id);
        var document = await Collection.Find(filter).FirstOrDefaultAsync();

        return document is null ? null : ToDomain(document);
    }

    public virtual async Task<TEntity?> GetByPublicIdAsync(string publicId)
    {
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, publicId);
        var document = await Collection.Find(filter).FirstOrDefaultAsync();

        return document is null ? null : ToDomain(document);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        var document = ToDocument(entity);
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, entity.Id);
        await Collection.ReplaceOneAsync(filter, document);

        await PublishEventsAsync(entity);
    }

    public virtual Task DeleteAsync(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(d => d.PublicId, id);
        return Collection.DeleteOneAsync(filter);
    }

    private Task PublishEventsAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        var events = entity.Events.ToList();
        entity.ClearEvents();

        return eventsDispatcher.DispatchAsync(events, cancellationToken);
    }
}