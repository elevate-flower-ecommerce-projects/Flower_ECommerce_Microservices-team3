namespace Payment_Service.Features.Commands.HandlePaymentWebhook
{
    public sealed record HandlePaymentWebhookRequest(
        string EventId,
        string EventType,
        string IntentionId,
        string TransactionId,
        decimal Amount,
        string Currency,
        bool Success,
        bool Pending,
        bool Cancelled
    );
}
