using Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.Commands;
using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.Validators
{
    public sealed class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
    {
        public UpdateStoreCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Store ID is required.");

            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name!)
                    .NotEmpty()
                    .WithMessage("Store name cannot be empty when provided.")
                    .MaximumLength(150)
                    .WithMessage("Store name cannot exceed 150 characters.");
            });

            When(x => x.Location != null, () =>
            {
                RuleFor(x => x.Location!.Lat)
                    .InclusiveBetween(-90.0, 90.0)
                    .WithMessage("Latitude must be between -90 and 90.");

                RuleFor(x => x.Location!.Lng)
                    .InclusiveBetween(-180.0, 180.0)
                    .WithMessage("Longitude must be between -180 and 180.");
            });
        }
    }
}
