using FluentValidation;
using Identity.Application.Features.Verify_OTP;

namespace Identity.Application.Features.Auth.VerifyOtp;

public sealed class VerifyOtpValidator
    : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email address.");

        RuleFor(x => x.Otp)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Matches(@"^\d{6}$")
            .WithMessage("Verification code must be exactly 6 digits.");
    }
}