using FluentValidation;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking;

public sealed class GetOrderTrackingOrchestratorValidator : AbstractValidator<GetOrderTrackingOrchestrator>
{
    public GetOrderTrackingOrchestratorValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}
