using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Identity.Application.Interfaces;
using Identity.Application.Settings;
using Identity.Domain.Entities;
using MediatR;
using Blocks.Domain.Errors;
using Identity.Application.Features.Login.Queries;
using Identity.Application.Features.RefreshTokens.Commands;
using Identity.Domain.Enums;
using Microsoft.Extensions.Options;
using Identity.Application.Features.Login.ViewModels;

namespace Identity.Application.Features.Login.Commands
{
    public class LoginOrchestratorHandler(
    IGenericRepository<User> userRepository,
    IPasswordService passwordService,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings,
    IMediator mediator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginOrchestrator, Result<LoginResponseVM>>
    {
        private const string GenericAuthError = "Invalid email or password.";
       
        public async Task<Result<LoginResponseVM>> Handle(LoginOrchestrator request ,CancellationToken cancellationToken)
         { 
            var normalizedEmail = request.Email.ToLowerInvariant();

            //check rate limit
            var isBlocked = await mediator.Send(new CheckRateLimitQuery(normalizedEmail, request.IpAddress), cancellationToken);
            if(isBlocked)
            {
                return Result.Failure<LoginResponseVM>
                    (Error.TooManyRequests("Too many failed login attempts. Please try again later."));
     
            }

            //find user and verify password
            var users = await userRepository.FindAsync(u => u.Email == normalizedEmail);
            var user = users.FirstOrDefault();
            if(user == null || !passwordService.Verify(request.Password ,user.HashPassword))
            {
                await mediator.Send(
                new LogLoginAttemptCommand(normalizedEmail, request.IpAddress, false),
                cancellationToken);
                return Result.Failure<LoginResponseVM>(Error.Unauthorized(GenericAuthError));

            }

            //check active status
            if(!user.IsActive)
            {
                return Result.Failure<LoginResponseVM>(Error.Unauthorized(GenericAuthError));
            }

            //check role
            if (user.Role != UserRole.Customer && user.Role != UserRole.Driver)
            {
                return Result.Failure<LoginResponseVM>(Error.Unauthorized(GenericAuthError));
            }
            //generate tokens
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var accessToken = tokenService.GenerateAccessToken(user);
                var refreshTokenValue = tokenService.GenerateRefreshToken();
               
                await mediator.Send(new SaveRefreshTokenCommand(
                    refreshTokenValue,
                    user.Id,
                    DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays)),
                    cancellationToken);
                
                await mediator.Send(
                    new LogLoginAttemptCommand(normalizedEmail, request.IpAddress, true),
                    cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);


                //response
                var expiresIn = jwtSettings.Value.AccessTokenExpirationMinutes * 60;
                string? driverStatus = null;
                return Result.Success(new LoginResponseVM(
                accessToken, refreshTokenValue, expiresIn,
                user.Role.ToString(), driverStatus));
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }


        }

    }
}
