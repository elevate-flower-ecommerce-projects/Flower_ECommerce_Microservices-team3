using FluentValidation;
using Identity.Application.Features.Drivers.Commands.SubmitDriverApplication;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Delivery.Commands.SubmitDriverApplication;

public sealed class SubmitDriverApplicationCommandValidator
    : AbstractValidator<SubmitDriverApplicationCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly string[] AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf"
    ];

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "application/pdf"
    ];

    public SubmitDriverApplicationCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.SecondName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.Gender)
            .IsInEnum();

        RuleFor(x => x.VehicleType)
            .IsInEnum();

        RuleFor(x => x.VehicleNumber)
            .NotEmpty();

        RuleFor(x => x.NationalId)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");

        // At least one file is required
        RuleFor(x => x)
            .Must(x =>
                x.VehicleLicenceFile is not null ||
                x.IdImage is not null)
            .WithMessage(
                "At least one vehicle licence or ID image is required.");

        // Validate licence only if provided
        RuleFor(x => x.VehicleLicenceFile)
            .Must(IsValidFile)
            .When(x => x.VehicleLicenceFile is not null)
            .WithMessage("Invalid vehicle licence file.");

        // Validate ID only if provided
        RuleFor(x => x.IdImage)
            .Must(IsValidFile)
            .When(x => x.IdImage is not null)
            .WithMessage("Invalid ID image file.");
    }

    private static bool IsValidFile(IFormFile file)
    {
        if (file.Length <= 0 || file.Length > MaxFileSize)
            return false;

        return AllowedExtensions.Contains(
                   Path.GetExtension(file.FileName),
                   StringComparer.OrdinalIgnoreCase)
               &&
               AllowedContentTypes.Contains(
                   file.ContentType,
                   StringComparer.OrdinalIgnoreCase);
    }
}