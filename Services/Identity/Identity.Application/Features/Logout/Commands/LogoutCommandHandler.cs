using Blocks.Contracts.Interfaces;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using Blocks.Contracts.Common;
using Blocks.Domain.Errors;

namespace Identity.Application.Features.Logout.Commands
{
    public class LogoutCommandHandler(
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IDeviceRegistrationService deviceRegistrationService,
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

                if (sessions.Count == 0)
                {
                    await unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Failure(
                        Error.NotFound("No active session was found for the supplied DeviceId."));
                }

                foreach (var session in sessions)
                {
                    session.IsRevoked = true;
                    session.RevokedAt ??= DateTime.UtcNow;
                }

                await deviceRegistrationService.UnregisterAsync(
                    request.UserId,
                    request.DeviceId,
                    cancellationToken);

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

