using FluentValidation;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands
{
    public class ConfirmOrderDeliveryCommandValidator
        : AbstractValidator<ConfirmOrderDeliveryCommand>
    {
        public ConfirmOrderDeliveryCommandValidator()
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
