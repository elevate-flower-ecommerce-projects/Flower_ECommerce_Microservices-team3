using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.RefreshTokens.Commands.CommandHandlers
{
    public class RevokeRefreshTokenCommandHandler(
        IGenericRepository<RefreshToken> refreshTokenRepository)
        : IRequestHandler<RevokeRefreshTokenCommand>
    {
        public async Task Handle(
            RevokeRefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var token = await refreshTokenRepository.GetByIdAsync(request.TokenId);

            if (token is null) return;

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            refreshTokenRepository.Update(token);
        }
    }
}
