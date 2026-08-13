using Blocks.Contracts.Common;
using Identity.Application.Features.Login.DTOs;
using MediatR;

namespace Identity.Application.Features.Login.Commands;

public sealed record LoginCommand(
    string Email,
    string Password,
    string IpAddress,
    string? DeviceId = null,
    string? FcmToken = null)
    : IRequest<Result<LoginResponseDto>>;
