using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Api.Features.Forgot_Password;
using Identity.Application.Features.ResetPassword;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordHandler(
    IUnitOfWork unitOfWork,
    IGenericRepository<PasswordResetToken> resetTokenRepository,
    IGenericRepository<User> userRepository,
    IGenericRepository<RefreshToken> refreshTokenRepository,
    IPasswordService passwordService,
    IResetTokenService resetTokenService,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<
        ResetPasswordCommand,
        Result<ResetPasswordResponse>>
{
    public async Task<Result<ResetPasswordResponse>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            resetTokenService.Hash(request.ResetToken);

        var resetToken =
            await resetTokenRepository.FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                x => x);

        if (resetToken is null)
        {
            return Result.Failure<ResetPasswordResponse>(
                Error.Unauthorized(
                    "AUTH_INVALID_RESET_TOKEN"));
        }

        var now = dateTimeProvider.UtcNow;

        if (resetToken.IsExpired(now))
        {
            return Result.Failure<ResetPasswordResponse>(
                Error.Unauthorized(
                    "AUTH_RESET_TOKEN_EXPIRED"));
        }

        if (resetToken.IsUsed())
        {
            return Result.Failure<ResetPasswordResponse>(
                Error.Unauthorized(
                    "AUTH_INVALID_RESET_TOKEN"));
        }

        // Get user
        var user =
            await userRepository.FirstOrDefaultAsync(
                x => x.Id == resetToken.UserId,
                x => x);

        if (user is null)
        {
            return Result.Failure<ResetPasswordResponse>(
                Error.NotFound(
                    "AUTH_USER_NOT_FOUND"));
        }

        // Update password
        user.HashPassword = passwordService.Hash(request.NewPassword);

        // Consume reset token
        resetToken.MarkAsUsed(now);

        // Invalidate all refresh tokens
        var refreshTokens =
            await refreshTokenRepository.FindAsync(
                x => x.UserId == resetToken.UserId);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.RevokedAt = now;
        }

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            new ResetPasswordResponse(
                "Password reset successfully. Please login again."));
    }
}