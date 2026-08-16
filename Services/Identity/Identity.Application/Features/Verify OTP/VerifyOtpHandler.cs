using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.Verify_OTP;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.Auth.VerifyOtp;

public sealed class VerifyOtpHandler(
    IUnitOfWork unitOfWork,
    IGenericRepository<User> userRepository,
    IGenericRepository<PasswordResetOtp> otpRepository,
    IGenericRepository<PasswordResetToken> resetTokenRepository,
    IOtpService otpService,
    IResetTokenService resetTokenService,
    IDateTimeProvider dateTimeProvider)
     :IRequestHandler<
        VerifyOtpCommand,
        Result<VerifyOtpResponse>>
{
    public async Task<Result<VerifyOtpResponse>> Handle(
        VerifyOtpCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(
            x => x.Email == request.Email,
            x => x);

        if (user is null)
        {
            return Result.Failure<VerifyOtpResponse>(
                Error.Unauthorized(
                    "AUTH_INVALID_OTP"));
        }

        var otp = await otpRepository.FirstOrDefaultAsync(
            x =>
                x.UserId == user.Id &&
                !x.IsUsed,
            x => x);

        if (otp is null)
        {
            return Result.Failure<VerifyOtpResponse>(
                Error.Unauthorized(
                    "AUTH_INVALID_OTP"));
        }

        var now = dateTimeProvider.UtcNow;

        // OTP expired
        if (otp.IsExpired(now))
        {
            otp.MarkAsUsed();

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            return Result.Failure<VerifyOtpResponse>(
                Error.Unauthorized(
                    "AUTH_OTP_EXPIRED"));
        }

        // Attempts exhausted
        if (otp.AttemptsRemaining <= 0)
        {
            return Result.Failure<VerifyOtpResponse>(
                Error.TooManyRequests(
                    "AUTH_OTP_ATTEMPTS_EXCEEDED"));
        }

        // Invalid OTP
        if (!otpService.Verify(
                request.Otp,
                otp.OtpHash))
        {
            otp.DecreaseAttempt();

            await unitOfWork.SaveChangesAsync(
                cancellationToken);

            if (otp.AttemptsRemaining <= 0)
            {
                return Result.Failure<VerifyOtpResponse>(
                    Error.TooManyRequests(
                        "AUTH_OTP_ATTEMPTS_EXCEEDED"));
            }

            return Result.Failure<VerifyOtpResponse>(
                Error.Unauthorized(
                    "AUTH_INVALID_OTP"));
        }

        // OTP is valid
        otp.MarkAsUsed();

        // Generate reset token
        var resetToken =
            resetTokenService.Generate();

        // Store only the hash
        var resetTokenHash =
            resetTokenService.Hash(resetToken);

        var passwordResetToken =
            new PasswordResetToken(
                user.Id,
                resetTokenHash,
                now);

        resetTokenRepository.Add(
            passwordResetToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            new VerifyOtpResponse(
                resetToken,
                passwordResetToken.ExpiresAt));
    }
}