using Catalog_Service.Features.Occasions.GetPaginatedOccasions.Queries;
using FluentValidation;

namespace Catalog_Service.Features.Occasions.GetPaginatedOccasions.Validators
{
    public class GetActiveOccasionsQueryValidator : AbstractValidator<GetActiveOccasionsQuery>
    {
        public GetActiveOccasionsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be at least 1.")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100.");
        }
    }
}
