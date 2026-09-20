using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.DTOs;
using Identity.Application.Features.AdminLogin.Commands;
using Identity.Application.Features.AdminLogin.ViewModels;
using Identity.Application.Features.RefreshTokens.Commands;
using Identity.Application.Interfaces;
using Identity.Application.Settings;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.AdminLogin.Commands.CommandHandlers;

public class AdminLoginOrchestratorHandler(
    IGenericRepository<User> userRepository,
    IPasswordService passwordService,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings,
    ILoginRateLimiter rateLimiter,
    IMediator mediator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AdminLoginOrchestrator, Result<AdminLoginResponseVm>>
{
    private const string GenericAuthError = "Invalid credentials.";

    public async Task<Result<AdminLoginResponseVm>> Handle(
        AdminLoginOrchestrator request,
        CancellationToken cancellationToken)
    {
        if (rateLimiter.IsBlocked(request.Email, request.IpAddress))
        {
            return Result.Failure<AdminLoginResponseVm>(
                Error.TooManyRequests("Too many failed attempts. Try again later."));
        }

        var emailLower = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.FirstOrDefaultAsync(
            u => u.Email == emailLower,
            u => new { u.Id, u.HashPassword, u.Role, u.IsActive }
        );

        AdminLoginOutcome? failureOutcome = null;

        if (user is null || !passwordService.Verify(request.Password, user.HashPassword))
        {
            failureOutcome = AdminLoginOutcome.InvalidCredentials;
        }
        else if (!user.IsActive)
        {
            failureOutcome = AdminLoginOutcome.AccountDisabled;
        }
        else if (user.Role != UserRole.Admin)
        {
            failureOutcome = AdminLoginOutcome.NotAdminRole;
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            if (failureOutcome.HasValue)
            {
                rateLimiter.RecordFailure(request.Email, request.IpAddress);
                await mediator.Send(new CreateAdminLoginAuditCommand(
                    request.Email, request.IpAddress, request.UserAgent, failureOutcome.Value),
                    cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Failure<AdminLoginResponseVm>(Error.Unauthorized(GenericAuthError));
            }

            rateLimiter.Reset(request.Email, request.IpAddress);

            var userDto = new UserTokenDto(user!.Id, emailLower, user.Role, user.IsActive, CustomerId: null);
            var accessToken = tokenService.GenerateAccessToken(userDto);
            var refreshTokenValue = tokenService.GenerateRefreshToken();

            var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.AccessTokenExpirationMinutes);

            await mediator.Send(new SaveRefreshTokenCommand(
                refreshTokenValue,
                user.Id,
                DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays)),
                cancellationToken);

            await mediator.Send(new CreateAdminLoginAuditCommand(
                request.Email, request.IpAddress, request.UserAgent, AdminLoginOutcome.Success),
                cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(new AdminLoginResponseVm(accessToken, refreshTokenValue, expiresAt));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
