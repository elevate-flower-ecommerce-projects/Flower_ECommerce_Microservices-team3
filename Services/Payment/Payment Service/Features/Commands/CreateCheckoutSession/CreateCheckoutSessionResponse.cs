using Blocks.Contracts.Payment;

namespace Payment_Service.Features.Commands.CreateCheckoutSession
{
    public sealed record CreateCheckoutSessionResponse(
        Guid OrderId,
        Guid PaymentId,
        string PaymentUrl,
        string SessionId,
        string SessionUrl,
        PaymentProvider PaymentProvider = PaymentProvider.Paymob,
        string Status = "Pending",
        string? SuccessUrl = null,
        string? CancelUrl = null,
        DateTime? ExpiresAt = null,
        decimal Amount = 0,
        string Currency = "EGP",
        DateTime? EstimatedDeliveryAt = null
    );
}
