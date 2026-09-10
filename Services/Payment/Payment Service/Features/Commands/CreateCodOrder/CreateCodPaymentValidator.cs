using FluentValidation;

namespace Payment_Service.Features.Commands.CreateCodPayment;

public sealed class CreateCodPaymentValidator
    : AbstractValidator<CreateCodPaymentCommand>
{
    public CreateCodPaymentValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a valid 3-letter currency code.");
    }
}