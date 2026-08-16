using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.RefreshTokens.Queries;

public sealed record RefreshTokenLookupDto(Guid Id, Guid UserId, string? DeviceId);

public sealed record GetRefreshTokenQuery(string Token) : IRequest<RefreshTokenLookupDto?>;

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
              && t.RevokedAt == null
              && t.ExpiresAt > DateTime.UtcNow,
            t => new RefreshTokenLookupDto(t.Id, t.UserId, t.DeviceId), cancellationToken);
    }
}
}
