using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.Commands
{
    public class LoginValidator : AbstractValidator<LoginOrchestrator>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
