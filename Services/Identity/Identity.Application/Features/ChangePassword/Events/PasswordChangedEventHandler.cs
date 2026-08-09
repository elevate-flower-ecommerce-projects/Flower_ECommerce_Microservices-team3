using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Features.ChangePassword.Events
{
    public class PasswordChangedEventHandler(IEmailService emailService)
        : INotificationHandler<PasswordChangedEvent>
    {
        public async Task Handle(PasswordChangedEvent notification, CancellationToken cancellationToken)
        {
            await emailService.SendPasswordChangedEmailAsync(
                notification.Email,
                notification.UserName,
                cancellationToken
            );
        }
    }
}
