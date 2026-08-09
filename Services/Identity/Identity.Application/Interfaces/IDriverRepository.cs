using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        Task<bool> ExistsByNationalIdAsync(
            string nationalId,
            CancellationToken cancellationToken = default);
    }
}
