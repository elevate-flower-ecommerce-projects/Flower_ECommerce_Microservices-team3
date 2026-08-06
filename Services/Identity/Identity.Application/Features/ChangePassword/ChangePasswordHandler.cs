using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.ChangePassword.Events;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;
namespace Identity.Application.Features.ChangePassword
{
    public class ChangePasswordHandler(
    IGenericRepository<User> userRepository,
    IPasswordService passwordService,
    ISessionService sessionService,
    IPublisher publisher,
    IUnitOfWork unitOfWork
   )
    : IRequestHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return Result.Failure(Error.NotFound("User was not found."));
                }

                bool isCurrentPasswordValid = passwordService.Verify(request.CurrentPassword, user.HashPassword);
                if (!isCurrentPasswordValid)
                {
                    return Result.Failure(Error.Unauthorized("Current password is incorrect."));
                }

                user.HashPassword = passwordService.Hash(request.NewPassword);
                userRepository.Update(user);
                await sessionService.RevokeAllUserSessionsAsync(request.UserId, cancellationToken);
               
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                await publisher.Publish(new PasswordChangedEvent(user.Email, user.FirstName), cancellationToken);
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
