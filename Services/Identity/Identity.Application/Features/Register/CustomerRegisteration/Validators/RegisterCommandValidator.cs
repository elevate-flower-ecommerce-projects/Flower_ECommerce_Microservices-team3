using FluentValidation;
using Identity.Application.Features.Register.CustomerRegisteration.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Register.CustomerRegisteration.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(\+20|0)?1[0-2,5][0-9]{8}$")
                .WithMessage("Phone number must be a valid Egyptian mobile number (e.g. 01012345678 or +201012345678).");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => !string.IsNullOrEmpty(g) && (g.Equals("Male", StringComparison.OrdinalIgnoreCase) || g.Equals("Female", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Gender must be either Male or Female.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");
        }
    }
}
