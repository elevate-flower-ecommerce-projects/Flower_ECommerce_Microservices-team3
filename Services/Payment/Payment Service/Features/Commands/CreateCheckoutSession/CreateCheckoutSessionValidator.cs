using FluentValidation;

namespace Payment_Service.Features.Commands.CreateCheckoutSession;

public sealed class CreateCheckoutSessionValidator
    : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3)
            .WithMessage("Currency is required.");

        RuleFor(x => x.EstimatedDeliveryAt)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Estimated delivery date must be in the future.");

        When(x => x.BillingData is not null, () =>
        {
            RuleFor(x => x.BillingData.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.BillingData.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.BillingData.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.BillingData.PhoneNumber)
                .NotEmpty()
                .MaximumLength(30);
        });
    }
}