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
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Estimated delivery date must be in the future.");

        RuleFor(x => x.BillingData)
            .NotNull()
            .WithMessage("Billing data is required.");

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

        RuleFor(x => x.BillingData.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BillingData.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BillingData.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BillingData.Building)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.BillingData.Floor)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.BillingData.Apartment)
            .NotEmpty()
            .MaximumLength(20);
    }
}