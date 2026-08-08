using Identity.Domain.Enums;
using MediatR;

namespace Identity.Application.Features.AdminLogin.Commands
{
    public sealed record CreateAdminLoginAuditCommand(
        string Email,
        string IpAddress,
        string UserAgent,
        AdminLoginOutcome Outcome)
        : IRequest;
}
