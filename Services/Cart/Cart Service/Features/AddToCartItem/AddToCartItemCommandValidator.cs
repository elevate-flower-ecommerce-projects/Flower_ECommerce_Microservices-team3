using Cart_Service.Features.AddToCartItem;
using FluentValidation;

namespace Cart_Service.Features.Cart.AddItem;

public sealed class AddToCartCommandValidator
    : AbstractValidator<AddToCartItemCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(50);

        RuleFor(x => x.Language)
            .NotEmpty()
            .MaximumLength(10);
    }
}