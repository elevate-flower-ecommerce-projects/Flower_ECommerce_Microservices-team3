namespace Payment_Service.Features.Commands.CreateCodPayment;

public sealed record CreateCodPaymentRequest(
    Guid OrderId,
    decimal Amount,
    string Currency
);