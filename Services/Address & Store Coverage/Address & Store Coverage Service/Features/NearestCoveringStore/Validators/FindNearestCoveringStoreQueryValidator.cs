using Address___Store_Coverage_Service.Features.NearestCoveringStore.Queries;
using FluentValidation;

namespace Address___Store_Coverage_Service.Features.NearestCoveringStore.Validators
{
    public sealed class FindNearestCoveringStoreQueryValidator
        : AbstractValidator<FindNearestCoveringStoreQuery>
    {
        public FindNearestCoveringStoreQueryValidator()
        {
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
        }
    }
}
