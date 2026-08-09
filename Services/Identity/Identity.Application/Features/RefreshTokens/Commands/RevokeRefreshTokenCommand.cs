using MediatR;

namespace Identity.Application.Features.RefreshTokens.Commands;

public sealed record RevokeRefreshTokenCommand(Guid TokenId) : IRequest;
