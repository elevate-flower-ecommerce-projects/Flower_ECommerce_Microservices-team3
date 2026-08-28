using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore.Commands
{
    public sealed class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
    {
        public CreateStoreCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Store name is required.")
                .MaximumLength(150)
                .WithMessage("Store name cannot exceed 150 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90.0, 90.0)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180.0, 180.0)
                .WithMessage("Longitude must be between -180 and 180.");
        }
    }
}
