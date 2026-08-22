using Identity.Application.Features.Login.Commands;
using Identity.Application.Features.Login.DTOs;
using MediatR;

namespace Identity.Api.Features.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            [Microsoft.AspNetCore.Mvc.FromBody] LoginRequestDto? request,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.BadRequest(new { error = "Request body is required." });
            }

            var ipAddress = GetClientIpAddress(httpContext);

            var result = await mediator.Send(
                new LoginCommand(
                    request.Email ?? string.Empty,
                    request.Password ?? string.Empty,
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
        .WithTags("Authentication")
        .AllowAnonymous()
        .Produces<LoginResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static string GetClientIpAddress(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var firstIp = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(firstIp))
            {
                // Remove port if present (e.g. 127.0.0.1:5292 or [::1]:5292)
                if (firstIp.StartsWith('[') && firstIp.Contains(']'))
                {
                    firstIp = firstIp.Substring(1, firstIp.IndexOf(']') - 1);
                }
                else if (firstIp.Count(c => c == ':') == 1)
                {
                    firstIp = firstIp.Split(':')[0];
                }

                return firstIp.Length > 45 ? firstIp[..45] : firstIp;
            }
        }

        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return remoteIp.Length > 45 ? remoteIp[..45] : remoteIp;
    }
}
