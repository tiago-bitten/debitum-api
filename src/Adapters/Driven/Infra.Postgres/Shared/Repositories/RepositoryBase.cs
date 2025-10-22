using Application.Shared.Ports;
using Domain.Shared.Entities;
using Infra.Postgres.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Postgres.Shared.Repositories;

internal abstract class RepositoryBase<TEntity>(DebitumDbContext context)
    : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly DebitumDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        // SaveChanges will be called by Unit of Work
    }

    public virtual async Task<TEntity?> GetByIdAsync(string id)
    {
        var entity = await DbSet.FirstOrDefaultAsync(e => e.Id == id);
        return entity;
    }

    public virtual async Task<TEntity?> GetByPublicIdAsync(string publicId)
    {
        var entity = await DbSet
            .FirstOrDefaultAsync(e => e.PublicId == publicId);

        return entity;
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        // SaveChanges will be called by Unit of Work
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(string id)
    {
        var entity = DbSet.Local.FirstOrDefault(e => e.Id == id);

        if (entity is not null)
        {
            DbSet.Remove(entity);
        }

        // SaveChanges will be called by Unit of Work
        return Task.CompletedTask;
    }
}
