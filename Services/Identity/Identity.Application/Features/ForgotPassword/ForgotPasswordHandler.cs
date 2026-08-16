using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.Auth.ForgotPassword;
using Identity.Application.Features.ForgotPassword;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IGenericRepository<PasswordResetOtp> otpRepository,
    IOtpService otpService,
    IEmailService emailService,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<
        ForgotPasswordCommand,
        Result<ForgotPasswordResponse>>
{
    public async Task<Result<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        const string message =
            "If this email is registered, a code has been sent.";

        var user = await userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        // Do not reveal whether the account exists.
        if (user is null)
        {
            return Result.Success(
                new ForgotPasswordResponse(message));
        }

        var now = dateTimeProvider.UtcNow;

        var lastOtp = await otpRepository.FirstOrDefaultAsync(
            x =>
                x.UserId == user.Id &&
                !x.IsUsed,
            x => x);

        // 30-second resend cooldown
        if (lastOtp is not null &&
            now < lastOtp.CreatedAt.AddSeconds(30))
        {
            return Result.Success(
                new ForgotPasswordResponse(message));
        }

        // Generate 6-digit OTP
        var otp = otpService.GenerateOtp();

        // Store only the hash
        var otpHash = otpService.Hash(otp);

        var passwordResetOtp = new PasswordResetOtp(
            user.Id,
            otpHash,
            now);

        otpRepository.Add(passwordResetOtp);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // Send OTP email
        //var emailResult = await emailService.SendOtpAsync(
        //    user.Email,
        //    otp,
        //    cancellationToken);

        //if (emailResult.IsFailure)
        //{
        //    return Result.Failure<ForgotPasswordResponse>(
        //        emailResult.Error);
        //}

        Console.BackgroundColor = ConsoleColor.Green;
        Console.WriteLine(otp);
        Console.ResetColor();

        return Result.Success(
            new ForgotPasswordResponse(message));
    }
}