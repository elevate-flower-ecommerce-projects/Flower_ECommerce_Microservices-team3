using Blocks.Contracts.Common;

namespace Identity.Application.Interfaces;

public interface IEmailService
{
    Task<Result> SendOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default);

    Task<Result> SendPasswordChangedEmailAsync(
        string toEmail,
        string userName,
        CancellationToken cancellationToken = default);
}