using Domain.Shared.Entities;

namespace Application.Shared.Ports;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task AddAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(string id);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(string id);
}