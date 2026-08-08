using FluentValidation;

namespace Identity.Api.Features.AdminLogin
{
    public sealed record AdminLoginRequestVm(string Email, string Password);

    public class AdminLoginRequestVmValidator : AbstractValidator<AdminLoginRequestVm>
    {
        public AdminLoginRequestVmValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
