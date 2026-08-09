using System;
using System.Collections.Generic;
using System.Text;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public class DriverApplicationRepository
        : GenericRepository<DriverApplication>,
          IDriverApplicationRepository
    {
        public DriverApplicationRepository(FlowersAuthDbContext context)
            : base(context)
        {
        }

        public async Task<DriverApplication?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DriverApplications
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);
        }
    }
}
