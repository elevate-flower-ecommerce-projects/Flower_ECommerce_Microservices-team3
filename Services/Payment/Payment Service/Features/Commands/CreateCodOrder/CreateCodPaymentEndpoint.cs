using MediatR;
using Payment_Service.Features.Commands.CreateCodPayment;

namespace Payment_Service.Features;

public static class CreateCodPaymentEndpoint
{
    public static IEndpointRouteBuilder MapCreateCodPayment(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/payments/cod",
            async (
                CreateCodPaymentRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCodPaymentCommand(
                    request.OrderId,
                    request.Amount,
                    request.Currency);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                return result;
            });

        return endpoints;
    }
}