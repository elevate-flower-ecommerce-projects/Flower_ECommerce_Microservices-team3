namespace Payment_Service.Features.Commands.CreateCheckoutSession
{
    public sealed record CreateCheckoutSessionResponse(
        Guid OrderId,
        Guid PaymentId,
        string PaymentUrl
    );
}
