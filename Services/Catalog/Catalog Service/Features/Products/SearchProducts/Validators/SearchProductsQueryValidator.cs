using Catalog_Service.Features.Products.SearchProducts.Queries;
using FluentValidation;

namespace Catalog_Service.Features.Products.SearchProducts.Validators
{
    public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
    {
        public SearchProductsQueryValidator()
        {
            RuleFor(x => x.Keyword)
                .NotEmpty()
                .WithMessage("Search keyword is required.")
                .MinimumLength(2)
                .WithMessage("Search keyword must be at least 2 characters long.");

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
