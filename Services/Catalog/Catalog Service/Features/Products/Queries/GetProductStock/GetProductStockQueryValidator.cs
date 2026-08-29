using FluentValidation;

namespace Catalog_Service.Features.Products.Queries.GetProductStock
{
    public sealed class GetProductStockQueryValidator
        : AbstractValidator<GetProductStockQuery>
    {
        public GetProductStockQueryValidator()
        {
            RuleFor(x => x.ProductIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one product ID is required.");

            RuleForEach(x => x.ProductIds)
                .NotEmpty()
                .WithMessage("Product ID cannot be empty.");
        }
    }
}
