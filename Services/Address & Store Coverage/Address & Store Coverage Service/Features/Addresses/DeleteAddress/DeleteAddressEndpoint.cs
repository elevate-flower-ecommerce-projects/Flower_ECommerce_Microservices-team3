using Address___Store_Coverage_Service.Features.Addresses.DeleteAddress.Commands;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Address___Store_Coverage_Service.Features.Addresses.DeleteAddress
{
    public static class DeleteAddressEndpoint
    {
        public static IEndpointRouteBuilder MapDeleteAddressEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/users/me/addresses/{id:guid}", async (
               Guid id,
               ClaimsPrincipal user,
               IMediator mediator,
               CancellationToken ct) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim)
                   || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(Error.Forbidden("Access denied.")),
                        statusCode: StatusCodes.Status403Forbidden);
                }
                var result = await mediator.Send(new DeleteAddressCommand(id, customerId), ct);
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
                return Results.Ok(ApiResponse<string>.Ok(result.Value, result.Value));
            })
            .WithName("DeleteAddress")
            .WithTags("Addresses")
            .RequireAuthorization();
            return app;
        }
    }
    } 

