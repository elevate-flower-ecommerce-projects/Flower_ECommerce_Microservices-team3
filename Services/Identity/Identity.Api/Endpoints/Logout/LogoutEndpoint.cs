using Identity.Application.Features.Logout.Commands;
using Identity.Application.Features.Logout.DTOs;
using MediatR;

namespace Identity.Api.Features.Logout
{
    public static class LogoutEndpoint
    {
        public static IEndpointRouteBuilder MapLogoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/logout", async (
                LogoutRequestDto request,
                IMediator mediator,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                if (request is null)
                {
                    return Results.Problem("Request body is required.", statusCode: StatusCodes.Status400BadRequest);
                }

                var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                   ?? httpContext.User.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await mediator.Send(
                    new LogoutCommand(userId, request.DeviceId),
                    cancellationToken);

                if (result.IsFailure)
                {
                    return Results.Json(
                        new { error = result.Error!.Message },
                        statusCode: result.Error.StatusCode);
                }

                return Results.NoContent();
            })
            .WithName("Logout")
            .WithTags("Authentication")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }
}
