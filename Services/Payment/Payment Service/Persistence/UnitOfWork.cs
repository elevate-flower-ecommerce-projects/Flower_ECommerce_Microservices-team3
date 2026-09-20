using Payment_Service.Application.Abstractions;

namespace Payment_Service.Persistence;

public sealed class UnitOfWork(FlowersPaymentDbContext _context) 
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
