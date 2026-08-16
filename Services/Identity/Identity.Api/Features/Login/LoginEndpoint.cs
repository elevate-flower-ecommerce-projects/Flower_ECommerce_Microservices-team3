using Identity.Application.Features.Login.Commands;
using Identity.Application.Features.Login.DTOs;
using MediatR;

namespace Identity.Api.Features.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            LoginRequestDto request,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "unknown";

            var result = await mediator.Send(
                new LoginCommand(
                    request.Email,
                    request.Password,
                    ipAddress,
                    request.DeviceId,
                    request.FcmToken),
                cancellationToken);

            if (result.IsFailure)
            {
                return Results.Json(
                    new { error = result.Error!.Message },
                    statusCode: result.Error.StatusCode);
            }

            return Results.Ok(result.Value);
        })
        .WithName("Login")
        .Produces<LoginResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }
}
