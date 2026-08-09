using FluentValidation;
using Identity.Application.Features.DriverApplication.SubmitApplication.Commands;
using Identity.Application.Features.DriverApplication.SubmitApplication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Features.DriverApplication;

public static class SubmitDriverApplicationEndpoint
{
    public static IEndpointRouteBuilder MapSubmitDriverApplicationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/drivers/applications", async (
            SubmitDriverApplicationDto request,
            IValidator<SubmitDriverApplicationDto> validator,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = new SubmitDriverApplicationCommand(request);
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return Results.Json(
                    new { error = result.Error!.Message },
                    statusCode: result.Error.StatusCode
                );
            }

            return Results.Created($"/api/drivers/applications/{result.Value.ApplicationId}", result.Value);
        })
        .WithName("SubmitDriverApplication")
        .WithTags("Driver Applications")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        return app;
    }
}
