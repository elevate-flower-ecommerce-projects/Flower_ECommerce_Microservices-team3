using Address___Store_Coverage_Service.Features.Addresses.GetAddressById.DTOs;
using Address___Store_Coverage_Service.Features.Addresses.GetAddressById.Queries;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.GetAddressById
{
    public static class GetAddressByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetAddressByIdEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/users/me/addresses/{id:guid}", async (
                Guid id,
                IMediator mediator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim)
                    || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<AddressDetailsDto>.Fail(
                           Error.Forbidden("Access denied.")),
                        statusCode: 403);
                }
                var result = await mediator.Send(
                    new GetAddressByIdQuery(customerId, id), ct);
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<AddressDetailsDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
                return Results.Ok(
                    ApiResponse<AddressDetailsDto>.Ok(result.Value));
            })
            .WithName("GetAddressById")
            .WithTags("Addresses")
            .RequireAuthorization();
            return app;
        }
    }
}
