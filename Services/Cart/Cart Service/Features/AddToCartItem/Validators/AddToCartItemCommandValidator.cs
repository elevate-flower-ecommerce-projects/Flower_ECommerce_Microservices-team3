using FluentValidation;

namespace Cart_Service.Features.AddToCartItem.Commands;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder validator for AddItemToCart (SCRUM-11 / SCRUM-87 / SCRUM-88).
// =========================================================================================================

public class AddToCartItemCommandValidator : AbstractValidator<AddToCartItemCommand>
{
    public AddToCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
