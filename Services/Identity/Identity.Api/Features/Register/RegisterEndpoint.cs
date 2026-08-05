using FluentValidation;
using Identity.Application.Features.Register.Commands;
using Identity.Application.Features.Register.DTOs;
using Identity.Domain.Enums;
using MediatR;

namespace Identity.Api.Features.Register
{
    public static class RegisterEndpoint
    {
        public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/register", async (
                RegisterRequestDto request,
                IValidator<RegisterRequestDto> validator,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                if (!Enum.TryParse<Gender>(request.Gender, true, out var parsedGender))
                {
                    return Results.BadRequest(new { error = "Invalid gender value. Must be Male or Female." });
                }

                var command = new RegisterCommand(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Password,
                    request.PhoneNumber,
                    parsedGender
                );

                var result = await mediator.Send(command, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.Json(
                        new { error = result.Error!.Message },
                        statusCode: result.Error.StatusCode
                    );
                }

                return Results.Created($"/auth/users/{result.Value.Id}", result.Value);
            })
            .WithName("RegisterCustomer")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

            return app;
        }
    }
}
