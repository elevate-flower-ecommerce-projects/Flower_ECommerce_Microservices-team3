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
            var billing = request.BillingData ?? new Blocks.Contracts.Payment.BillingData(
                !string.IsNullOrWhiteSpace(request.CustomerFirstName) ? request.CustomerFirstName : "Customer",
                !string.IsNullOrWhiteSpace(request.CustomerLastName) ? request.CustomerLastName : "User",
                !string.IsNullOrWhiteSpace(request.CustomerEmail) ? request.CustomerEmail : "customer@example.com",
                !string.IsNullOrWhiteSpace(request.CustomerPhone) ? request.CustomerPhone : "+201000000000",
                !string.IsNullOrWhiteSpace(request.Country) ? request.Country : "EGY",
                !string.IsNullOrWhiteSpace(request.City) ? request.City : "Cairo",
                !string.IsNullOrWhiteSpace(request.Street) ? request.Street : "123 Street",
                !string.IsNullOrWhiteSpace(request.Building) ? request.Building : "1",
                !string.IsNullOrWhiteSpace(request.Floor) ? request.Floor : "1",
                !string.IsNullOrWhiteSpace(request.Apartment) ? request.Apartment : "1"
            );

            var eta = request.EstimatedDeliveryAt.HasValue && request.EstimatedDeliveryAt.Value > DateTime.UtcNow
                ? request.EstimatedDeliveryAt.Value
                : DateTime.UtcNow.AddMinutes(45);

            var command = new CreateCheckoutSessionCommand(
                request.OrderId,
                request.Amount,
                string.IsNullOrWhiteSpace(request.Currency) ? "EGP" : request.Currency,
                eta,
                billing);

            var result = await sender.Send(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(FloweryApiResponse<CreateCheckoutSessionResponse>.Success(
                    result.Value,
                    "Checkout session created successfully.",
                    "تم إنشاء جلسة الدفع بنجاح.",
                    "Created"));
            }

            var statusCode = result.Error?.StatusCode switch
            {
                null or 0 => StatusCodes.Status400BadRequest,
                var code => code
            };

            return Results.Json(
                FloweryApiResponse<CreateCheckoutSessionResponse>.Failure(
                    result.Error?.Message ?? "Failed to create checkout session.",
                    statusCode == 409 ? "Conflict" : "BadRequest",
                    result.Error?.Message),
                statusCode: statusCode);
        };

        endpoints.MapPost("/payments/checkout-session", handler)
            .WithName("CreateCheckoutSession")
            .WithTags("Payments")
            .Produces<FloweryApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status200OK)
            .Produces<FloweryApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status400BadRequest)
            .Produces<FloweryApiResponse<CreateCheckoutSessionResponse>>(StatusCodes.Status409Conflict);

        endpoints.MapPost("/payments/sessions", handler)
            .WithName("CreatePaymentSession")
            .WithTags("Payments");

        endpoints.MapPost("/checkout-session", handler)
            .WithName("CreateGatewayCheckoutSession")
            .WithTags("Payments");

        return endpoints;
    }
}