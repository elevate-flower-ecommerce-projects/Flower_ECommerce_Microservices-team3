using FluentValidation;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public sealed class GetCheckoutDetailsValidator : AbstractValidator<GetCheckoutDetailsQuery>
{
    public GetCheckoutDetailsValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");
    }
}
