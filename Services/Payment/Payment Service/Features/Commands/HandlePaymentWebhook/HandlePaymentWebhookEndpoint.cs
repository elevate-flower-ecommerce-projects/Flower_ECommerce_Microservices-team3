using System.Text.Json;
using Blocks.Contracts.Http;
using MediatR;

namespace Payment_Service.Features.Commands.HandlePaymentWebhook;

public static class HandlePaymentWebhookEndpoint
{
    public static IEndpointRouteBuilder MapHandlePaymentWebhook(
        this IEndpointRouteBuilder endpoints)
    {
        var handler = async (
            JsonDocument jsonDoc,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var root = jsonDoc.RootElement;
            string eventId = string.Empty;
            string eventType = "TRANSACTION";
            string intentionId = string.Empty;
            string transactionId = string.Empty;
            decimal amount = 0;
            string currency = "EGP";
            bool success = false;
            bool pending = false;
            bool cancelled = false;

            // Check if Paymob native format { "type": "TRANSACTION", "obj": { ... } }
            if (root.TryGetProperty("obj", out var obj))
            {
                if (root.TryGetProperty("type", out var typeEl))
                {
                    eventType = typeEl.GetString() ?? "TRANSACTION";
                }

                if (obj.TryGetProperty("id", out var idEl))
                {
                    transactionId = idEl.ToString();
                    eventId = $"tx_{transactionId}";
                }

                if (obj.TryGetProperty("order", out var orderEl))
                {
                    intentionId = orderEl.ToString();
                }

                if (obj.TryGetProperty("success", out var successEl) && successEl.ValueKind is JsonValueKind.True or JsonValueKind.False)
                {
                    success = successEl.GetBoolean();
                }

                if (obj.TryGetProperty("pending", out var pendingEl) && pendingEl.ValueKind is JsonValueKind.True or JsonValueKind.False)
                {
                    pending = pendingEl.GetBoolean();
                }

                if (obj.TryGetProperty("amount_cents", out var amountEl))
                {
                    if (amountEl.TryGetInt64(out var cents))
                    {
                        amount = cents / 100m;
                    }
                }

                if (obj.TryGetProperty("currency", out var currEl))
                {
                    currency = currEl.GetString() ?? "EGP";
                }
            }
            else
            {
                // Fallback to HandlePaymentWebhookRequest format
                var request = JsonSerializer.Deserialize<HandlePaymentWebhookRequest>(
                    root.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (request is not null)
                {
                    eventId = request.EventId;
                    eventType = request.EventType;
                    intentionId = request.IntentionId;
                    transactionId = request.TransactionId;
                    amount = request.Amount;
                    currency = request.Currency;
                    success = request.Success;
                    pending = request.Pending;
                    cancelled = request.Cancelled;
                }
            }

            var command = new HandlePaymentWebhookCommand(
                eventId,
                eventType,
                intentionId,
                transactionId,
                amount,
                currency,
                success,
                pending,
                cancelled);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<string>.Ok("Webhook processed successfully"));
            }

            var statusCode = result.Error?.StatusCode switch
            {
                null or 0 => StatusCodes.Status400BadRequest,
                var code => code
            };

            return Results.Json(
                ApiResponse<string>.Fail(result.Error!),
                statusCode: statusCode);
        };

        endpoints.MapPost("/api/v1/payments/webhook", handler)
            .WithName("HandlePaymentWebhookV1")
            .WithTags("Payments");

        endpoints.MapPost("/payments/webhook", handler)
            .WithName("HandlePaymentWebhook")
            .WithTags("Payments");

        endpoints.MapPost("/webhook", handler)
            .WithName("HandleWebhook")
            .WithTags("Payments");

        return endpoints;
    }
}