using FluentValidation;

namespace Identity.Application.Features.Login.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");


        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("DeviceId is required when an FcmToken is supplied.")
            .When(x => !string.IsNullOrWhiteSpace(x.FcmToken));

        
        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FcmToken is required when a DeviceId is supplied.")
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));

       
        RuleFor(x => x.DeviceId)
            .MaximumLength(128).WithMessage("DeviceId must not exceed 128 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));

        RuleFor(x => x.FcmToken)
            .MaximumLength(512).WithMessage("FcmToken must not exceed 512 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FcmToken));
    }
}
