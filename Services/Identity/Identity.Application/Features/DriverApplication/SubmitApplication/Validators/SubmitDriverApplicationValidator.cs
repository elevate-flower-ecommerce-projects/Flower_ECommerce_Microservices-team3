using FluentValidation;
using Identity.Application.Features.DriverApplication.SubmitApplication.DTOs;

namespace Identity.Application.Features.DriverApplication.SubmitApplication.Validators;

public class SubmitDriverApplicationValidator : AbstractValidator<SubmitDriverApplicationDto>
{
    public SubmitDriverApplicationValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^01[0-2,5][0-9]{8}$")
            .WithMessage("Phone number must be a valid Egyptian mobile number (11 digits starting with 010, 011, 012, or 015).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");

        RuleFor(x => x.VehicleNumber)
            .NotEmpty().WithMessage("Vehicle plate number is required.");

        RuleFor(x => x.NationalIdNumber)
            .NotEmpty().WithMessage("National ID is required.")
            .Length(14).WithMessage("National ID must be 14 digits.");

        RuleFor(x => x.VehicleLicenceImage)
            .NotEmpty().WithMessage("Vehicle license document image/file is required.");

        RuleFor(x => x.NationalIdImage)
            .NotEmpty().WithMessage("National ID document image/file is required.");
    }
}
