using FluentValidation;
using Identity.Application.Features.ForgotPassword;

namespace Identity.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")

            .EmailAddress()
            .WithMessage("Invalid email address.");
    }
}