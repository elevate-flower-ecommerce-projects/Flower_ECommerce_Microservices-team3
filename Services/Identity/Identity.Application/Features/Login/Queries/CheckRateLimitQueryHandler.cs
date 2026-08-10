using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Identity.Application.Features.Login.Queries
{
    public class CheckRateLimitQueryHandler(IGenericRepository<LoginAttempt> loginAttemptRepository) 
        : IRequestHandler<CheckRateLimitQuery, bool>
    {
        private const int MaxFailedAttempts = 5;
        private const int LockoutWindowMinutes = 15;

        public async Task<bool> Handle(CheckRateLimitQuery request, CancellationToken cancellationToken)
        {
            var windowStart = DateTime.UtcNow.AddMinutes(-LockoutWindowMinutes);

            var normalizedEmail = request.Email.ToLowerInvariant();
            var failedByEmail = await loginAttemptRepository.FindAsync(
           a => a.Email == normalizedEmail
             && !a.IsSuccessful
             && a.AttemptedAt >= windowStart);
            if (failedByEmail.Count() >= MaxFailedAttempts)
                return true;

            if (!string.IsNullOrEmpty(request.IpAddress) && request.IpAddress != "unknown")
            {
                var failedByIp = await loginAttemptRepository.FindAsync(
                    a => a.IpAddress == request.IpAddress
                      && !a.IsSuccessful
                      && a.AttemptedAt >= windowStart);

                if (failedByIp.Count() >= MaxFailedAttempts * 3)
                    return true;
            }

            return false;

        }
    }
}
