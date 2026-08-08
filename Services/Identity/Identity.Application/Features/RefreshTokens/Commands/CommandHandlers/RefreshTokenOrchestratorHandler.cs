using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.DTOs;
using Identity.Application.Features.RefreshTokens.Queries;
using Identity.Application.Features.RefreshTokens.ViewModels;
using Identity.Application.Interfaces;
using Identity.Application.Settings;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.RefreshTokens.Commands.CommandHandlers
{
    public class RefreshTokenOrchestratorHandler(
        IGenericRepository<User> userRepository,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        IMediator mediator,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RefreshTokenOrchestrator, Result<RefreshTokenResponseVm>>
    {
        public async Task<Result<RefreshTokenResponseVm>> Handle(
            RefreshTokenOrchestrator request,
            CancellationToken cancellationToken)
        {
            var existingToken = await mediator.Send(
                new GetRefreshTokenQuery(request.Token), cancellationToken);

            if (existingToken is null)
                return Result.Failure<RefreshTokenResponseVm>(
                    Error.Unauthorized("Invalid or expired refresh token."));

            var user = await userRepository.FirstOrDefaultAsync(
                u => u.Id == existingToken.UserId,
                u => new UserTokenDto(u.Id, u.Email, u.Role, u.IsActive));

            if (user is null || !user.IsActive)
                return Result.Failure<RefreshTokenResponseVm>(
                    Error.Unauthorized("Invalid or expired refresh token."));

            var newRefreshTokenValue = tokenService.GenerateRefreshToken();
            var newAccessToken = tokenService.GenerateAccessToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.AccessTokenExpirationMinutes);

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await mediator.Send(
                    new RevokeRefreshTokenCommand(existingToken.Id), cancellationToken);

                await mediator.Send(new SaveRefreshTokenCommand(
                    newRefreshTokenValue,
                    user.Id,
                    DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays)),
                    cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return Result.Success(new RefreshTokenResponseVm(newAccessToken, newRefreshTokenValue, expiresAt));
        }
    }
}
