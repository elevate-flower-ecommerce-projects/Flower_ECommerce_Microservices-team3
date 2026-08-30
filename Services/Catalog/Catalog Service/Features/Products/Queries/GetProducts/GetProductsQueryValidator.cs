using FluentValidation;

namespace Catalog_Service.Features.Products.Queries.GetProducts
{
    public sealed class GetProductsQueryValidator
        : AbstractValidator<GetProductsQuery>
    {
        public GetProductsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.CategoryId)
                .NotEqual(Guid.Empty)
                .When(x => x.CategoryId.HasValue)
                .WithMessage("CategoryId must be a valid GUID.");

            RuleFor(x => x.OccasionId)
                .NotEqual(Guid.Empty)
                .When(x => x.OccasionId.HasValue)
                .WithMessage("OccasionId must be a valid GUID.");

            RuleFor(x => x.StoreId)
                .NotEqual(Guid.Empty)
                .When(x => x.StoreId.HasValue)
                .WithMessage("StoreId must be a valid GUID.");

            RuleFor(x => x.Sort)
                .IsInEnum()
                .When(x => x.Sort.HasValue)
                .WithMessage("Sort must be a valid product sort option.");
        }
    }
}