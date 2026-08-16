using Catalog_Service.Features.Products.GetProductsByOccasionId.Queries;
using FluentValidation;

namespace Catalog_Service.Features.Products.GetProductsByOccasionId.Validators
{
    public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
    {
        public GetProductsQueryValidator()
        {
            RuleFor(x => x.OccasionId)
                .NotEmpty()
                .WithMessage("Occasion Id is required to filter products.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be at least 1.");
        }
    }
}
