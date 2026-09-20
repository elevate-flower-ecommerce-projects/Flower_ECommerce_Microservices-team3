using Blocks.Contracts.Interfaces;
using Blocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Order___Fulfillment_Service.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly FlowersOrderDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(FlowersOrderDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdatePartial(T entity, params Expression<Func<T, object>>[] updatedProperties)
    {
        _context.Attach(entity);

        var entry = _context.Entry(entity);
        foreach (var property in updatedProperties)
        {
            entry.Property(property).IsModified = true;
        }
    }

    public void SaveInclude(T entity, params string[] includedProperties)
    {
        var localEntity = _dbSet.Local.FirstOrDefault(e => ((dynamic)e).Id == ((dynamic)entity).Id);
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry;

        if (localEntity == null)
        {
            entry = _context.Entry(entity);
        }
        else
        {
            entry = _context.ChangeTracker.Entries<T>().First(e => ((dynamic)e.Entity).Id == ((dynamic)entity).Id);
        }

        foreach (var property in entry.Properties)
        {
            if (property.Metadata.IsPrimaryKey())
            {
                continue;
            }
            else
            {
                if (includedProperties.Contains(property.Metadata.Name))
                {
                    property.IsModified = true;
                }
                else
                {
                    property.IsModified = false;
                }
            }
        }
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
