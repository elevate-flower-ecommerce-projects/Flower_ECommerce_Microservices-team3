using System.Linq.Expressions;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Payment_Service.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : BaseEntity
    {
        protected readonly FlowersPaymentDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FlowersPaymentDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<TResult?> FirstOrDefaultAsync<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdatePartial(
            T entity,
            params Expression<Func<T, object>>[] updatedProperties)
        {
            _context.Attach(entity);

            foreach (var property in updatedProperties)
            {
                _context.Entry(entity)
                    .Property(property)
                    .IsModified = true;
            }
        }

        public void SaveInclude(
            T entity,
            params string[] includedProperties)
        {
            _dbSet.Attach(entity);

            var entry = _context.Entry(entity);

            foreach (var property in includedProperties)
            {
                entry.Property(property).IsModified = true;
            }
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
