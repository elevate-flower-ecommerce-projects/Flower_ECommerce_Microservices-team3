using FluentValidation;
using Identity.Api.Features.Forgot_Password;

namespace Identity.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordValidator
    : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")

            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")

            .MaximumLength(100)
            .WithMessage("Password must not exceed 100 characters.")

            .Matches("[A-Z]")
            .WithMessage(
                "Password must contain at least one uppercase letter.")

            .Matches("[a-z]")
            .WithMessage(
                "Password must contain at least one lowercase letter.")

            .Matches("[0-9]")
            .WithMessage(
                "Password must contain at least one number.")

            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage(
                "Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")

            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}