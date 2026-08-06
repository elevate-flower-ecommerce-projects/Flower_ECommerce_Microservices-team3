using Identity.Application.Interfaces;
using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Services
{
    public class SessionService(IGenericRepository<RefreshToken> refreshTokenRepository) : ISessionService
    {
        public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var activeTokens = await refreshTokenRepository
                .FindAsync(t => t.UserId == userId && t.RevokedAt == null);
            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }
        }
    }
}
