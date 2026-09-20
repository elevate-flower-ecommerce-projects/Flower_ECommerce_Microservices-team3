using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class DriverRepository
    : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(FlowersAuthDbContext context)
        : base(context)
    {
    }

    public async Task<Driver?> GetByIdAsNoTrackingAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == driverId,
                cancellationToken);
    }

    public async Task<bool> ExistsByNationalIdAsync(
        string nationalId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .AnyAsync(
                x => x.NationalIdNumber == nationalId,
                cancellationToken);
    }
}