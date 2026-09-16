using FluentValidation;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery
{
    public class ConfirmDeliveryOrchestratorValidator
     : AbstractValidator<ConfirmDeliveryOrchestrator>
    {
        public ConfirmDeliveryOrchestratorValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("OrderId is required.");
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required.");
        }
    }

}
