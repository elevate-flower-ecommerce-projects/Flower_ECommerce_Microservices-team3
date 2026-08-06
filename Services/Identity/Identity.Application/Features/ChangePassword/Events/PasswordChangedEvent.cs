using MediatR;

namespace Identity.Application.Features.ChangePassword.Events
{
    public record PasswordChangedEvent(
        string Email,
        string UserName
    ) : INotification;
}
