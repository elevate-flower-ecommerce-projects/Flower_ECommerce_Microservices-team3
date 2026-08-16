using FluentValidation;
using Identity.Application.Features.AdminLogin.Commands;

namespace Identity.Application.Features.AdminLogin.Validators;

public class AdminLoginOrchestratorValidator : AbstractValidator<AdminLoginOrchestrator>
{
    public AdminLoginOrchestratorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
