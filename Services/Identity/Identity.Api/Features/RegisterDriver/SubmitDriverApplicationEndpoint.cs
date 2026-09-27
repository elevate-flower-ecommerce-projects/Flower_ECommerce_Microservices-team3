using Blocks.Contracts.Http;
using Identity.Application.Features.Drivers.Commands.SubmitDriverApplication;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Features.RegisterDriver;

public static class SubmitDriverApplicationEndpoint
{
    public static void MapSubmitDriverApplicationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/drivers/applications",
            async (
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
            ISender mediator,
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
                              IdImage: idImage);

                var result = await mediator.Send(command, cancellationToken);

                return result;
            })
        .DisableAntiforgery()
        .WithName("SubmitDriverApplication")
        .WithTags("Drivers")
        .WithSummary("Submit Driver Application")
        .Produces<ApiResponse<SubmitDriverApplicationResponse>>(201)
        .Produces<ApiResponse<SubmitDriverApplicationResponse>>(400)
        .Produces<ApiResponse<SubmitDriverApplicationResponse>>(409);
    }
}