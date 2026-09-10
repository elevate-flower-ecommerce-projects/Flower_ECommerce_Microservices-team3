using Blocks.Contracts.Payment;

namespace Payment_Service.Features.Commands.CreateCheckoutSession
{
    public sealed record CreateCheckoutSessionRequest(
        Guid OrderId,
        decimal Amount,
        string Currency,
        DateTime EstimatedDeliveryAt,
        BillingData BillingData
    );
}
