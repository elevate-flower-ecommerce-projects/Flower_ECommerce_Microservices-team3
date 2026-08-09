using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces
{
    public interface IDriverApplicationRepository
        : IGenericRepository<DriverApplication>
    {
        Task<DriverApplication?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
