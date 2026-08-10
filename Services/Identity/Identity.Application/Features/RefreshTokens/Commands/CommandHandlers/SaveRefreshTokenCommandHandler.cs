using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.RefreshTokens.Commands.CommandHandlers;

public class SaveRefreshTokenCommandHandler(
    IGenericRepository<RefreshToken> refreshTokenRepository)
    : IRequestHandler<SaveRefreshTokenCommand>
{
    public async Task Handle(SaveRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            Token = request.Token,
            UserId = request.UserId,
            ExpiresAt = request.ExpiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        });
    }
}
