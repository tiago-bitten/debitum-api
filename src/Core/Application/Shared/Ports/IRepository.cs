using Domain.Shared.Entities;

namespace Application.Shared.Ports;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
}