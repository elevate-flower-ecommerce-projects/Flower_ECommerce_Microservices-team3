using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress
{
    public static class CreateAddressEndpoint
    {
        public static IEndpointRouteBuilder MapCreateAddressEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/users/me/addresses", async (
                CreateAddressRequestDto request,
                IMediator mediator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim)
                    || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<CreateAddressResponseDto>.Fail(
                           Error.Forbidden("Access denied.")),
                        statusCode: 403);
                }
                var command = new CreateAddressOrchestrator(
                    CustomerId: customerId,
                    RecipientName: request.RecipientName,
                    Phone: request.Phone,
                    AddressLine: request.AddressLine,
                    CityId: request.CityId,
                    AreaId: request.AreaId,
                    Latitude: request.Latitude,
                    Longitude: request.Longitude,
                    Label: request.Label);
                var result = await mediator.Send(command, ct);
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<CreateAddressResponseDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
                return Results.Json(
                    ApiResponse<CreateAddressResponseDto>.Ok(
                        result.Value, "Address created successfully."),
                    statusCode: 201);
            })
            .WithName("CreateAddress")
            .WithTags("Addresses")
            .RequireAuthorization();
            return app;
        }
    }
}
