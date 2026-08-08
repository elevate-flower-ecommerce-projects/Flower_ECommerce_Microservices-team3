using FluentValidation;
using Identity.Application.Features.AdminLogin.Commands;

namespace Identity.Application.Features.AdminLogin.Validators
{
    public class AdminLoginOrchestratorValidator : AbstractValidator<AdminLoginOrchestrator>
    {
        public AdminLoginOrchestratorValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
