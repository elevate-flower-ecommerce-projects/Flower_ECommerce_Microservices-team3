using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.AdminLogin.Commands.CommandHandlers
{
    public class CreateAdminLoginAuditCommandHandler(
        IGenericRepository<AdminLoginAudit> auditRepository)
        : IRequestHandler<CreateAdminLoginAuditCommand>
    {
        public async Task Handle(CreateAdminLoginAuditCommand request, CancellationToken cancellationToken)
        {
            await auditRepository.AddAsync(new AdminLoginAudit
            {
                Email = request.Email.ToLowerInvariant(),
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Outcome = request.Outcome
            });
        }
    }
}
