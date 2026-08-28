using Address___Store_Coverage_Service.Entities;
using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.Validators
{
    public sealed class SetCoverageAreaCommandValidator : AbstractValidator<Commands.SetCoverageAreaCommand>
    {
        public SetCoverageAreaCommandValidator()
        {
            RuleFor(x => x.StoreId)
                .NotEmpty()
                .WithMessage("Store ID is required.");

            RuleFor(x => x.BoundaryType)
                .IsInEnum()
                .WithMessage("Invalid boundary type.");

            When(x => x.BoundaryType == CoverageBoundaryType.Polygon, () =>
            {
                RuleFor(x => x.Polygon)
                    .NotNull()
                    .WithMessage("Polygon coordinates are required when boundary type is Polygon.")
                    .Must(p => p != null && p.Count >= 3)
                    .WithMessage("Polygon must have at least 3 points.");

                RuleForEach(x => x.Polygon)
                    .ChildRules(point =>
                    {
                        point.RuleFor(p => p.Lat)
                            .InclusiveBetween(-90.0, 90.0)
                            .WithMessage("Polygon point latitude must be between -90 and 90.");

                        point.RuleFor(p => p.Lng)
                            .InclusiveBetween(-180.0, 180.0)
                            .WithMessage("Polygon point longitude must be between -180 and 180.");
                    });

                RuleFor(x => x.Polygon)
                    .Must(p =>
                    {
                        if (p is null || p.Count < 3) return true;
                        var first = p[0];
                        var last = p[^1];
                        return Math.Abs(first.Lat - last.Lat) < 0.000001 && Math.Abs(first.Lng - last.Lng) < 0.000001;
                    })
                    .WithMessage("A polygon boundary must be closed (first and last coordinates must match).");
            });

            When(x => x.BoundaryType == CoverageBoundaryType.Radius, () =>
            {
                RuleFor(x => x.RadiusMeters)
                    .NotNull()
                    .WithMessage("Radius in meters is required when boundary type is Radius.")
                    .GreaterThan(0)
                    .WithMessage("Radius in meters must be greater than 0.");
            });

            When(x => x.BoundaryType == CoverageBoundaryType.CityAreaList, () =>
            {
                RuleFor(x => x)
                    .Must(x => (x.Cities != null && x.Cities.Count > 0) || (x.Areas != null && x.Areas.Count > 0))
                    .WithMessage("At least one city or area must be specified when boundary type is CityAreaList.");
            });
        }
    }
}
