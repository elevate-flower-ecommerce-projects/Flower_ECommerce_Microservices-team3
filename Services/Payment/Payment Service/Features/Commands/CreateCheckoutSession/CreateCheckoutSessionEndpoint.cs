using MediatR;
using Payment_Service.Features.Commands.CreateCheckoutSession;

namespace Payment_Service.Features;

public static class CreateCheckoutSessionEndpoint
{
    public static IEndpointRouteBuilder MapCreateCheckoutSession(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/payments/checkout-session",
            async (
                CreateCheckoutSessionRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCheckoutSessionCommand(
                    request.OrderId,
                    request.Amount,
                    request.Currency,
                    request.EstimatedDeliveryAt,
                    request.BillingData);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                return result;
            });

        return endpoints;
    }
}