using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Vehicle.UpdateVehicle;

public sealed class UpdateVehicleValidator
    : AbstractValidator<UpdateVehicleCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    public UpdateVehicleValidator()
    {
        RuleFor(x => x.VehicleType)
            .IsInEnum()
            .WithMessage("Invalid vehicle type.");

        RuleFor(x => x.VehicleNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LicenseDocument)
            .NotNull()
            .WithMessage("License document is required.")
            .Must(file => file is not null && file.Length > 0)
            .WithMessage("License document cannot be empty.")
            .Must(file => file is null || file.Length <= MaxFileSize)
            .WithMessage("License document size must not exceed 5 MB.")
            .Must(file => file is null || IsAllowedExtension(file))
            .WithMessage("Only JPG, JPEG, PNG and PDF files are allowed.");
    }

    private static bool IsAllowedExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);

        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }
}