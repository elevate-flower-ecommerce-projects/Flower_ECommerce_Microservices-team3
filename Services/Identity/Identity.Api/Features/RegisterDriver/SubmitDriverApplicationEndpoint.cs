using Blocks.Contracts.Http;
using Identity.Application.Features.Drivers.Commands.SubmitDriverApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Features.RegisterDriver
{
    public static class SubmitDriverApplicationEndpoint
    {
        public static IEndpointRouteBuilder MapSubmitDriverApplicationEndpoint(
            this IEndpointRouteBuilder app)
        {
            var handler = async (
                [FromForm] SubmitDriverApplicationRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new SubmitDriverApplicationCommand(
                    CountryCode: request.CountryCode,
                    FirstName: request.FirstName,
                    SecondName: request.SecondName,
                    VehicleType: request.VehicleType,
                    VehicleNumber: request.VehicleNumber,
                    Email: request.Email,
                    PhoneNumber: request.PhoneNumber,
                    NationalId: request.NationalId,
                    Password: request.Password,
                    ConfirmPassword: request.ConfirmPassword,
                    Gender: request.Gender,
                    VehicleLicenceFile: request.VehicleLicenceFile,
                    IdImage: request.IdImage
                );

                var result = await mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Created(
                        $"/api/drivers/applications/{result.Value.ApplicationId}",
                        ApiResponse<SubmitDriverApplicationResponse>.Ok(result.Value, "Driver application submitted successfully."));
                }

                return Results.Json(
                    ApiResponse<SubmitDriverApplicationResponse>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            };

            app.MapPost("/api/drivers/applications", handler)
                .DisableAntiforgery()
                .WithName("SubmitDriverApplication")
                .WithTags("Drivers")
                .WithSummary("Submit Driver Application (Multipart Form)")
                .WithDescription("Submit a new driver application with personal details, vehicle info, national ID, and file uploads.")
                .Produces<ApiResponse<SubmitDriverApplicationResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<SubmitDriverApplicationResponse>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<SubmitDriverApplicationResponse>>(StatusCodes.Status409Conflict);

            app.MapPost("/api/v1/drivers/applications", handler)
                .DisableAntiforgery()
                .WithName("SubmitDriverApplicationV1")
                .WithTags("Drivers")
                .WithSummary("Submit Driver Application (v1 Multipart Form)")
                .Produces<ApiResponse<SubmitDriverApplicationResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<SubmitDriverApplicationResponse>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<SubmitDriverApplicationResponse>>(StatusCodes.Status409Conflict);

            return app;
        }
    }
}
