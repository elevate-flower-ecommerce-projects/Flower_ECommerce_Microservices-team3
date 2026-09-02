using Blocks.Contracts.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Order___Fulfillment_Service.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly FlowersOrderDbContext _dbContext;
    private readonly Stack<string> _savepoints = new();
    private int _depth;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(FlowersOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await ExecuteInternalAsync(async () =>
        {
            await action();
            return true;
        }, cancellationToken);
    }

    public Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        return ExecuteInternalAsync(action, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        GC.SuppressFinalize(this);
    }

    private async Task<T> ExecuteInternalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        var isOutermost = _depth == 0;

        if (isOutermost)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
        else
        {
            var savepointName = $"SP_Depth{_depth}_{Guid.NewGuid():N}"[..8];
            _savepoints.Push(savepointName);
            await _transaction!.CreateSavepointAsync(savepointName, cancellationToken);
        }

        _depth++;

        try
        {
            var result = await action();

            var shouldRollback = result is Result r && r.IsFailure;

            _depth--;

            if (shouldRollback)
            {
                if (_depth == 0)
                {
                    await _transaction!.RollbackAsync(cancellationToken);
                }
                else if (_savepoints.Count > 0)
                {
                    var savepoint = _savepoints.Pop();
                    await _transaction!.RollbackToSavepointAsync(savepoint, cancellationToken);
                }
            }
            else
            {
                if (_depth == 0)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await _transaction!.CommitAsync(cancellationToken);
                }
                else
                {
                    _savepoints.Pop();
                }
            }

            return result;
        }
        catch
        {
            _depth--;

            if (_depth == 0)
            {
                await _transaction!.RollbackAsync(cancellationToken);
            }
            else if (_savepoints.Count > 0)
            {
                var savepoint = _savepoints.Pop();
                await _transaction!.RollbackToSavepointAsync(savepoint, cancellationToken);
            }

            throw;
        }
        finally
        {
            if (_depth == 0 && _transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
                _savepoints.Clear();
            }
        }
    }
}
