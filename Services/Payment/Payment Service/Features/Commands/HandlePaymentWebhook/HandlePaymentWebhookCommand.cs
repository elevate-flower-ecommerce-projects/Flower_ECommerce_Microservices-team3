using Blocks.Contracts.Common;
using MediatR;

namespace Payment_Service.Features.Commands.HandlePaymentWebhook
{
    public sealed record HandlePaymentWebhookCommand(
        string EventId,
        string EventType,
        string IntentionId,
        string TransactionId,
        decimal Amount,
        string Currency,
        bool Success,
        bool Pending,
        bool Cancelled
    ) : IRequest<Result>;
}
