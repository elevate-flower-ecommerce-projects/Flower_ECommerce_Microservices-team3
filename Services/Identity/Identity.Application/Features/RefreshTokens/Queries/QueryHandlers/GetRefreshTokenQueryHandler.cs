using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.RefreshTokens.Queries.QueryHandlers
{
    public class GetRefreshTokenQueryHandler(
        IGenericRepository<RefreshToken> refreshTokenRepository)
        : IRequestHandler<GetRefreshTokenQuery, RefreshTokenLookupDto?>
    {
        public async Task<RefreshTokenLookupDto?> Handle(
            GetRefreshTokenQuery request,
            CancellationToken cancellationToken)
        {
            return await refreshTokenRepository.FirstOrDefaultAsync(
                t => t.Token == request.Token
                  && !t.IsRevoked
                  && t.ExpiresAt > DateTime.UtcNow,
                t => new RefreshTokenLookupDto(t.Id, t.UserId, t.DeviceId));
        }
    }
}
