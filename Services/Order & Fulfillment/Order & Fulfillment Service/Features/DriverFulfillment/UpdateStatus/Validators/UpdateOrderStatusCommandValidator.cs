using FluentValidation;
using Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.Commands;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.Validators;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid driver status update.");
    }
}
