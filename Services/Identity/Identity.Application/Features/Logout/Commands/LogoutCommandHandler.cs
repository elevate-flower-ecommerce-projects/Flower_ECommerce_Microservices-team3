using Blocks.Contracts.Interfaces;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Common;
using Blocks.Domain.Errors;



namespace Identity.Application.Features.Logout.Commands
{
    public class LogoutCommandHandler(
    IGenericRepository<RefreshToken> refreshTokenRepository,
    IGenericRepository<UserDevice> userDeviceRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutCommand, Result>
    {
        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var sessions = await refreshTokenRepository.FindAsync(
                    t => t.UserId == request.UserId
                         && t.DeviceId == request.DeviceId
                         && (!t.IsRevoked || t.RevokedAt == null),
                    cancellationToken);

                var devices = await userDeviceRepository.FindAsync(
                    d => d.UserId == request.UserId && d.DeviceId == request.DeviceId,
                    cancellationToken);

               
                if (sessions.Count == 0 && devices.Count == 0)
                {
                    await unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Failure(
                        Error.NotFound("No active session or registered device was found for the supplied DeviceId."));
                }

                foreach (var session in sessions)
                {
                    session.IsRevoked = true;
                    session.RevokedAt ??= DateTime.UtcNow;
                }

                foreach (var device in devices)
                {
                    userDeviceRepository.Delete(device);
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
