using Blocks.Contracts.Http;
using Identity.Application.Features.Drivers.Commands.SubmitDriverApplication;
using Identity.Domain.Enums;
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
                [FromForm] string countryCode,
                [FromForm] string firstName,
                [FromForm] string secondName,
                [FromForm] VehicleType vehicleType,
                [FromForm] string vehicleNumber,
                [FromForm] string email,
                [FromForm] string phoneNumber,
                [FromForm] string nationalId,
                [FromForm] string password,
                [FromForm] string confirmPassword,
                [FromForm] Gender gender,
                IFormFile? vehicleLicenceFile,
                IFormFile? idImage,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new SubmitDriverApplicationCommand(
                    CountryCode: countryCode,
                    FirstName: firstName,
                    SecondName: secondName,
                    VehicleType: vehicleType,
                    VehicleNumber: vehicleNumber,
                    Email: email,
                    PhoneNumber: phoneNumber,
                    NationalId: nationalId,
                    Password: password,
                    ConfirmPassword: confirmPassword,
                    Gender: gender,
                    VehicleLicenceFile: vehicleLicenceFile,
                    IdImage: idImage
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
