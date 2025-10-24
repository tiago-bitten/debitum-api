using Application.Shared.Ports;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Postgres.Shared.Persistence;

internal sealed class UnitOfWork(DebitumDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public bool HasActiveTransaction => _transaction is not null;

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var result = await context.SaveChangesAsync(cancellationToken);

        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        return result;
    }

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            return;
        }

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}