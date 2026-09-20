using Blocks.Contracts.Http;
using MediatR;
using Payment_Service.Features.Commands.CreateCheckoutSession;

namespace Payment_Service.Features;

public static class CreateCheckoutSessionEndpoint
{
    public static IEndpointRouteBuilder MapCreateCheckoutSession(
        this IEndpointRouteBuilder endpoints)
    {
        var handler = async (
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

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CreateCheckoutSessionResponse>.Ok(result.Value));
            }

            var statusCode = result.Error?.StatusCode switch
            {
                null or 0 => StatusCodes.Status400BadRequest,
                var code => code
            };

            return Results.Json(
                ApiResponse<CreateCheckoutSessionResponse>.Fail(result.Error!),
                statusCode: statusCode);
        };

        endpoints.MapPost("/payments/checkout-session", handler)
            .WithName("CreateCheckoutSession")
            .WithTags("Payments")
            .Produces<ApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status409Conflict);

        endpoints.MapPost("/payments/sessions", handler)
            .WithName("CreatePaymentSession")
            .WithTags("Payments")
            .Produces<ApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status409Conflict);

        endpoints.MapPost("/checkout-session", handler)
            .WithName("CreateGatewayCheckoutSession")
            .WithTags("Payments");

        return endpoints;
    }
}