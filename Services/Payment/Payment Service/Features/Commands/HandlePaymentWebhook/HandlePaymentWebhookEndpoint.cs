using MediatR;

namespace Payment_Service.Features.Commands.HandlePaymentWebhook;

public static class HandlePaymentWebhookEndpoint
{
    public static IEndpointRouteBuilder MapHandlePaymentWebhook(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/payments/webhook",
            async (
                HandlePaymentWebhookRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new HandlePaymentWebhookCommand(
                    request.EventId,
                    request.EventType,
                    request.IntentionId,
                    request.TransactionId,
                    request.Amount,
                    request.Currency,
                    request.Success,
                    request.Pending,
                    request.Cancelled);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                return result;
            });

        return endpoints;
    }
}