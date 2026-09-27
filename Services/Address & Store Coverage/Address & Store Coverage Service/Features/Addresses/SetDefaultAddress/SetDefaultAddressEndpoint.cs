using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.Commands;
using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress
{
    public static class SetDefaultAddressEndpoint
    {
        public static IEndpointRouteBuilder MapSetDefaultAddressEndpoint(this IEndpointRouteBuilder app)
        {
            var handler = async (
                Guid id,
                IMediator mediator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
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
                        statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
                }

                return Results.Ok(
                    ApiResponse<SetDefaultAddressResponseDto>.Ok(
                        result.Value, "Default address updated successfully."));
            };

            app.MapPatch("/users/me/addresses/{id:guid}/default", handler)
                .WithName("SetDefaultAddress")
                .WithTags("Addresses")
                .WithSummary("Set Default Address (PATCH)")
                .Produces<ApiResponse<SetDefaultAddressResponseDto>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<SetDefaultAddressResponseDto>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<SetDefaultAddressResponseDto>>(StatusCodes.Status404NotFound)
                .RequireAuthorization();

            // Flexible method and path aliases for Flutter and API Gateway
            app.MapPut("/users/me/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPost("/users/me/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();

            app.MapPatch("/api/users/me/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPut("/api/users/me/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPost("/api/users/me/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();

            app.MapPatch("/api/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPut("/api/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPost("/api/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();

            app.MapPatch("/api/v1/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPut("/api/v1/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPost("/api/v1/addresses/{id:guid}/default", handler).ExcludeFromDescription().RequireAuthorization();

            return app;
        }
    }
}
