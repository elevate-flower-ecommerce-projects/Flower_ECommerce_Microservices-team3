using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Identity.Infrastructure.Services;

public sealed class EmailService(IOptions<EmailSettings> options)
    : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task<Result> SendOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject = "Password Reset OTP";

            message.Body = new TextPart("plain")
            {
                Text = $"""
                    Your password reset code is: {otp}

                    This code expires in 10 minutes.

                    If you did not request a password reset,
                    you can safely ignore this email.
                    """
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.SmtpPort,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await client.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.Password,
                cancellationToken);

            await client.SendAsync(
                message,
                cancellationToken);

            await client.DisconnectAsync(
                true,
                cancellationToken);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(
                Error.Internal(
                    "AUTH_EMAIL_SEND_FAILED"));
        }
    }

    public async Task<Result> SendPasswordChangedEmailAsync(
        string toEmail,
        string userName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            message.To.Add(
                new MailboxAddress(
                    userName,
                    toEmail));

            message.Subject = "Your Password Was Changed";

            message.Body = new TextPart("html")
            {
                Text = $"""
                <h2>Hi {userName},</h2>

                <p>
                    Your password was successfully changed.
                </p>

                <p>
                    If you did not make this change,
                    please contact support immediately.
                </p>

                <br/>

                <p>
                    — Flower App Team
                </p>
                """
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.SmtpPort,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await client.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.Password,
                cancellationToken);

            await client.SendAsync(
                message,
                cancellationToken);

            await client.DisconnectAsync(
                true,
                cancellationToken);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(
                Error.Internal(
                    "AUTH_PASSWORD_CHANGED_EMAIL_FAILED"));
        }
    }
}