using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Validators
{
    public sealed class CreateAddressOrchestratorValidator
       : AbstractValidator<CreateAddressOrchestrator>
    {
        public CreateAddressOrchestratorValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.");
            RuleFor(x => x.RecipientName)
                .NotEmpty().WithMessage("Recipient name is required.")
                .MaximumLength(100);
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^01[0125]\d{8}$")
                .WithMessage("Phone must be a valid Egyptian mobile number.");
            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Address line is required.")
                .MaximumLength(250);
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.AreaId)
                .NotEmpty().WithMessage("Area is required.");
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90d, 90d)
                .WithMessage("Latitude must be between -90 and 90.");
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180d, 180d)
                .WithMessage("Longitude must be between -180 and 180.");
            RuleFor(x => x)
                .Must(x => x.Latitude != 0d || x.Longitude != 0d)
                .WithName("Coordinates")
                .WithMessage("Coordinates are required.");
            RuleFor(x => x.Label)
                .MaximumLength(50);
        }
    }
}
