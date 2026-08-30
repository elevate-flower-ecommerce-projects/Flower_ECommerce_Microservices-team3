using FluentValidation;

namespace Catalog_Service.Features.Products.Queries.GetProductsPage;

public sealed class GetProductsPageQueryValidator
    : AbstractValidator<GetProductsPageQuery>
{
    public GetProductsPageQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.StoreId)
            .NotEqual(Guid.Empty)
            .When(x => x.StoreId.HasValue);

        RuleFor(x => x.Sort)
            .IsInEnum()
            .When(x => x.Sort.HasValue);
    }
}