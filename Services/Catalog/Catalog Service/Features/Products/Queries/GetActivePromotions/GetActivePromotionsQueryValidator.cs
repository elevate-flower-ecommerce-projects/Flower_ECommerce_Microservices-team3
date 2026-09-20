using FluentValidation;

namespace Catalog_Service.Features.Products.Queries.GetActivePromotions
{
    public sealed class GetActivePromotionsQueryValidator
        : AbstractValidator<GetActivePromotionsQuery>
    {
        public GetActivePromotionsQueryValidator()
        {
            RuleFor(x => x.ProductIds)
                .NotEmpty()
                .WithMessage("At least one product ID is required.");

            RuleForEach(x => x.ProductIds)
                .NotEmpty()
                .WithMessage("Product ID cannot be empty.");
        }
    }
}
