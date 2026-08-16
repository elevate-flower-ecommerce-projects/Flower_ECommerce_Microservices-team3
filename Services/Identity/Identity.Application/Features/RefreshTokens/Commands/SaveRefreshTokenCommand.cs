using MediatR;

namespace Identity.Application.Features.RefreshTokens.Commands;

public sealed record SaveRefreshTokenCommand(
    string Token,
    Guid UserId,
    DateTime ExpiresAt,
    string? DeviceId = null
) : IRequest;
