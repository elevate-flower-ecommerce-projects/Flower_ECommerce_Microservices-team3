using MediatR;

namespace Identity.Application.Features.RefreshTokens.Queries
{
    public sealed record RefreshTokenLookupDto(Guid Id, Guid UserId);

    public sealed record GetRefreshTokenQuery(string Token) : IRequest<RefreshTokenLookupDto?>;
}
