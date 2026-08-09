using Identity.Application.Interfaces;
using Identity.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Services
{
    public class EmailService(IOptions<EmailSettings> options) : IEmailService
    {
        private readonly EmailSettings _settings = options.Value;
        public async Task SendPasswordChangedEmailAsync(string toEmail, string userName, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(userName, toEmail));
            message.Subject = "Your password was changed";
            message.Body = new TextPart("html")
            {
                Text = $"""
                <h2>Hi {userName},</h2>
                <p>Your password was successfully changed on <strong>{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC</strong>.</p>
                
                <br/>
                <p>— Flower App Team</p>
                """
            };
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
