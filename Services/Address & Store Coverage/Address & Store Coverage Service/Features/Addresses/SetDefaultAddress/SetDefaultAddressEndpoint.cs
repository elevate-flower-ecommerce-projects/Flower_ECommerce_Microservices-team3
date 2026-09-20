using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.Commands;
using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress
{
    public static class SetDefaultAddressEndpoint
    {
        public static IEndpointRouteBuilder MapSetDefaultAddressEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPatch("/users/me/addresses/{id:guid}/default", async (
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
                        ApiResponse<SetDefaultAddressResponseDto>.Fail(
                           Error.Forbidden("Access denied.")),
                        statusCode: 403);
                }

                var command = new SetDefaultAddressCommand(customerId, id);
                var result = await mediator.Send(command, ct);
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<SetDefaultAddressResponseDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(
                    ApiResponse<SetDefaultAddressResponseDto>.Ok(
                        result.Value, "Default address updated successfully."));
            })
            .WithName("SetDefaultAddress")
            .WithTags("Addresses")
            .RequireAuthorization();

            return app;
        }
    }
}
