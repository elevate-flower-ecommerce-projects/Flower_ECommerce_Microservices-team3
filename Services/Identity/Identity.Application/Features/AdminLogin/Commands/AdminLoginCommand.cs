using Blocks.Contracts.Common;
using Identity.Application.Features.AdminLogin.ViewModels;
using MediatR;

namespace Identity.Application.Features.AdminLogin.Commands
{
    public sealed record AdminLoginOrchestrator(
        string Email,
        string Password,
        string IpAddress,
        string UserAgent)
        : IRequest<Result<AdminLoginResponseVm>>;
}
