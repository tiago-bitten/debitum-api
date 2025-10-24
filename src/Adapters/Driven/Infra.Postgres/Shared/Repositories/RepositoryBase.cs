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

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellation = default)
    {
        return await DbSet.FindAsync([id], cancellationToken: cancellation);
    }

    public virtual Task<TEntity?> GetByPublicIdAsync(string publicId, CancellationToken cancellation = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.PublicId == publicId, cancellation);
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(Guid id)
    {
        var entity = DbSet.Local.FirstOrDefault(e => e.Id == id);

        if (entity is not null)
        {
            DbSet.Remove(entity);
        }

        return Task.CompletedTask;
    }
}