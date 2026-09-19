using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Features.Drivers.ReportLocation.DTOs;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation
{
    public static class ReportDriverLocationEndpoint
    {
        public static IEndpointRouteBuilder MapReportDriverLocationEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/drivers/me/location", async (
                    ReportLocationRequest request,
                    ClaimsPrincipal user,
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
            {
               
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(
                            Error.Unauthorized("Invalid or missing user identity.")),
                        statusCode: StatusCodes.Status401Unauthorized);
                }
               
                var role = user.FindFirstValue(FlowerClaimTypes.Role) ?? user.FindFirstValue(ClaimTypes.Role);
                if (role != "Driver")
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(
                            Error.Forbidden("Only drivers can report location.")),
                        statusCode: StatusCodes.Status403Forbidden);
                }
              
                var orchestrator = new ReportDriverLocationOrchestrator(
                    userId,
                    request.Lat,
                    request.Lng,
                    request.RecordedAt
                );
                var result = await mediator.Send(orchestrator, cancellationToken);
              
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(ApiResponse<string>.Fail(result.Error!));
                }
                if (result.Value == null)
                {
                    return Results.NoContent(); 
                }
                return Results.Ok(ApiResponse<string>.Ok(result.Value)); 
            })
                .WithName("ReportDriverLocation")
                .WithTags("Drivers")
                .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<string>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<string>>(StatusCodes.Status403Forbidden)
                .RequireAuthorization();
            return app;
        }
    }
}
