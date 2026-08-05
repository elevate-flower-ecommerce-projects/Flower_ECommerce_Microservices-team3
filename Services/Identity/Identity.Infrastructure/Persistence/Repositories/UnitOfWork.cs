using Identity.Application.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork(FlowersAuthDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _currentTransaction;
        private static readonly AsyncLocal<int> _transactionDepth = new();

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionDepth.Value == 0)
            {
                _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
            }
            _transactionDepth.Value++;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_transactionDepth.Value > 1)
            {
                return 0; 
            }

            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transactionDepth.Value--;

            if (_transactionDepth.Value == 0)
            {
                try
                {
                    await context.SaveChangesAsync(cancellationToken);

                    if (_currentTransaction != null)
                    {
                        await _currentTransaction.CommitAsync(cancellationToken);
                    }
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
                finally
                {
                    if (_currentTransaction != null)
                    {
                        await _currentTransaction.DisposeAsync();
                        _currentTransaction = null;
                    }
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            _transactionDepth.Value = 0;

            if (_currentTransaction != null)
            {
                try
                {
                    await _currentTransaction.RollbackAsync();
                }
                finally
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }
    }
}
