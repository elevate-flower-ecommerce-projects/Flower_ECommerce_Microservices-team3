using Blocks.Contracts.Http;
using MediatR;
using Payment_Service.Features.Commands.CreateCodPayment;

namespace Payment_Service.Features;

public static class CreateCodPaymentEndpoint
{
    public static IEndpointRouteBuilder MapCreateCodPayment(
        this IEndpointRouteBuilder endpoints)
    {
        var handler = async (
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

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CreateCodPaymentResponse>.Ok(result.Value));
            }

            var statusCode = result.Error?.StatusCode switch
            {
                null or 0 => StatusCodes.Status400BadRequest,
                var code => code
            };

            return Results.Json(
                ApiResponse<CreateCodPaymentResponse>.Fail(result.Error!),
                statusCode: statusCode);
        };

        endpoints.MapPost("/payments/cod", handler)
            .WithName("CreateCodPayment")
            .WithTags("Payments")
            .Produces<ApiResponse<CreateCodPaymentResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CreateCodPaymentResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<CreateCodPaymentResponse>>(StatusCodes.Status409Conflict);

        endpoints.MapPost("/cod", handler)
            .WithName("CreateGatewayCodPayment")
            .WithTags("Payments");

        return endpoints;
    }
}