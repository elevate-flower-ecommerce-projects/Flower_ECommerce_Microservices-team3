using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress
{
    public static class UpdateAddressEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateAddressEndpoint(
            this IEndpointRouteBuilder app)
        {
            app.MapPut("/users/me/addresses/{id:guid}", async (
                Guid id,
                UpdateAddressRequestDto request,
                ClaimsPrincipal user,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim)
                    || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<UpdateAddressResponseDto>.Fail(
                            Error.Forbidden("Access denied.")),
                        statusCode: StatusCodes.Status403Forbidden);
                }
                var orchestrator = new UpdateAddressOrchestrator(
                    AddressId: id,
                    CustomerId: customerId,
                    RecipientName: request.RecipientName,
                    Phone: request.Phone,
                    AddressLine: request.AddressLine,
                    CityId: request.CityId,
                    AreaId: request.AreaId,
                    Latitude: request.Latitude,
                    Longitude: request.Longitude,
                    Label: request.Label
                );
                var result = await mediator.Send(orchestrator, ct);
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<UpdateAddressResponseDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
                return Results.Ok(
                    ApiResponse<UpdateAddressResponseDto>.Ok(
                        result.Value, "Address updated successfully"));
            })
            .WithName("UpdateAddress")
            .WithTags("Addresses")
            .RequireAuthorization();
            return app;
        }
    }
}
