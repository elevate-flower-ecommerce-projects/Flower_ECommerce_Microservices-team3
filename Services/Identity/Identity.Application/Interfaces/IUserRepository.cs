using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsByPhoneAsync(
            string phone,
            CancellationToken cancellationToken = default);
    }
}
