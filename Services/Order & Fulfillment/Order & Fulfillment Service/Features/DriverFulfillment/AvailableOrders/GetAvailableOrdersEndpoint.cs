using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.Queries;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders;

public static class GetAvailableOrdersEndpoint
{
    public static IEndpointRouteBuilder MapGetAvailableOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/drivers/available-orders", async (
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetAvailableOrdersQuery(page ?? 1, pageSize ?? 10);
            var result = await mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<AvailableOrderListDto>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<AvailableOrderListDto>.Ok(result.Value));
        })
        .WithName("GetAvailableOrdersForDriver")
        .WithTags("Driver Fulfillment")
        .RequireAuthorization(FlowerClaimTypes.DriverPolicy);

        return app;
    }
}
