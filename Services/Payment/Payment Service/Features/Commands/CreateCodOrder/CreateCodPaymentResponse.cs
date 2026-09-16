using Payment_Service.Entities.Enums;

namespace Payment_Service.Features.Commands.CreateCodPayment;

public sealed record CreateCodPaymentResponse(
    Guid OrderId,
    Guid PaymentId,
    PaymentStatus Status
);