using FluentValidation;
using Identity.Application.Features.AdminLogin.Commands;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Features.AdminLogin;

public static class AdminLoginEndpoint
{
    public static IEndpointRouteBuilder MapAdminLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/admin-login", async (
            AdminLoginRequestVm request,
            IValidator<AdminLoginRequestVm> validator,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            var result = await mediator.Send(
                new AdminLoginOrchestrator(request.Email, request.Password, ipAddress, userAgent),
                cancellationToken);

            if (result.IsFailure)
            {
                return Results.Json(
                    new { error = result.Error!.Message },
                    statusCode: result.Error.StatusCode);
            }

            return Results.Ok(result.Value);
        })
        .WithName("AdminLogin")
        .WithTags("Authentication")
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }
}
