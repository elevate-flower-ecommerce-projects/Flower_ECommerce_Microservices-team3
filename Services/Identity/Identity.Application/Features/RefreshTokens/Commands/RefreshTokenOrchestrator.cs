using Blocks.Contracts.Common;
using Identity.Application.Features.RefreshTokens.ViewModels;
using MediatR;

namespace Identity.Application.Features.RefreshTokens.Commands;

public sealed record RefreshTokenOrchestrator(string Token)
    : IRequest<Result<RefreshTokenResponseVm>>;
