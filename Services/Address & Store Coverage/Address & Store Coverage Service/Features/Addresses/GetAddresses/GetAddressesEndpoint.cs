using Address___Store_Coverage_Service.Features.Addresses.GetAddresses.DTOs;
using Address___Store_Coverage_Service.Features.Addresses.GetAddresses.Queries;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.GetAddresses
{
    public static class GetAddressesEndpoint
    {
        public static IEndpointRouteBuilder MapGetAddressesEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/users/me/addresses", async (
                IMediator mediator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim)
                    || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<List<AddressItemDto>>.Fail(
                           Error.Forbidden("Access denied.")),
                        statusCode: 403);
                }
                var result = await mediator.Send(
                    new GetAddressesQuery(customerId), ct);
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<List<AddressItemDto>>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
                return Results.Ok(
                    ApiResponse<List<AddressItemDto>>.Ok(result.Value));
            })
            .WithName("GetAddresses")
            .WithTags("Addresses")
            .RequireAuthorization();
            return app;
        }
    }
}
