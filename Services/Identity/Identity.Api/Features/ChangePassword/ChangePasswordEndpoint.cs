using Identity.Application.Features.ChangePassword;
using Identity.Application.Features.ChangePassword.DTOs;
using MediatR;
using System.Security.Claims;

namespace Identity.Api.Features.ChangePassword
{
    public static class ChangePasswordEndpoint
    {
        public static IEndpointRouteBuilder MapChangePasswordEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/change-password", async (
                ChangePasswordRequestDto request,
                IMediator mediator,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    return Results.BadRequest(new { error = "The new password and confirmation password do not match." });
                }

                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? httpContext.User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new ChangePasswordCommand(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword
                );

                var result = await mediator.Send(command, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.Json(
                        new { error = result.Error!.Message },
                        statusCode: result.Error.StatusCode
                    );
                }

                return Results.Ok(new { message = "Password changed successfully." });
            })
            .WithName("ChangePassword")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}
