namespace Application.Shared.Ports;

public interface IUnitOfWork
{
    bool HasActiveTransaction { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task BeginAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}