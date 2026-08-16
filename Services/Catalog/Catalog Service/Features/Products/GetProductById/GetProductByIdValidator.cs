using FluentValidation;

namespace Catalog_Service.Features.Products.GetProductById;

public sealed class GetProductByIdValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Language)
            .Must(lang => lang is "ar" or "en")
            .WithMessage("Language must be 'ar' or 'en'.");
    }
}
