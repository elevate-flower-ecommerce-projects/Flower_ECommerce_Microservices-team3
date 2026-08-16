using System;
using System.Collections.Generic;
using System.Text;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public class DriverRepository
        : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(FlowersAuthDbContext context)
            : base(context)
        {
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
}
