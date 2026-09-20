using FluentValidation;

namespace Order___Fulfillment_Service.Features.Checkout.EstimateDelivery;

public sealed class EstimateDeliveryValidator : AbstractValidator<EstimateDeliveryQuery>
{
    public EstimateDeliveryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.AddressId)
            .NotEmpty()
            .WithMessage("Address ID is required.");
    }
}
