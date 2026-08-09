using FluentValidation;

namespace Identity.Api.Features.AdminLogin;

public sealed record AdminLoginRequestVm(string Email, string Password);

public class AdminLoginRequestVmValidator : AbstractValidator<AdminLoginRequestVm>
{
    public AdminLoginRequestVmValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
