using Blocks.Contracts.Interfaces;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.Commands
{
    public class LogLoginAttemptCommandHandler(IGenericRepository<LoginAttempt> loginAttemptRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogLoginAttemptCommand>
    {
        public async Task Handle(
        LogLoginAttemptCommand request,
        CancellationToken cancellationToken)
        {
            await loginAttemptRepository.AddAsync(new LoginAttempt
            {
                Email = request.Email.ToLowerInvariant(),
                IpAddress = request.IpAddress,
                IsSuccessful = request.IsSuccessful,
                AttemptedAt = DateTime.UtcNow
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
