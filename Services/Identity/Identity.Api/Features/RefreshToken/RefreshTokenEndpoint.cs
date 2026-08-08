using FluentValidation;
using Identity.Application.Features.RefreshTokens.Commands;
using MediatR;

namespace Identity.Api.Features.RefreshToken
{
    public static class RefreshTokenEndpoint
    {
        public static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/refresh", async (
                RefreshTokenRequestVm request,
                IValidator<RefreshTokenRequestVm> validator,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var result = await mediator.Send(
                    new RefreshTokenOrchestrator(request.Token), cancellationToken);

                if (result.IsFailure)
                    return Results.Json(
                        new { error = result.Error!.Message },
                        statusCode: result.Error.StatusCode);

                return Results.Ok(result.Value);
            })
            .WithName("RefreshToken")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}
