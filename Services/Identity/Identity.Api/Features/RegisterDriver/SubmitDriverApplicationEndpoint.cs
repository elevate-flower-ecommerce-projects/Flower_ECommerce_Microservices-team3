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
            app.MapPost("/api/v1/drivers/applications",
                async (
                    [FromForm] SubmitDriverApplicationRequest request,
                    IMediator _mediator,
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

                    var result = await _mediator.Send(
                        command,
                        cancellationToken);

                    return result;
                })
                .DisableAntiforgery()
                .WithName("SubmitDriverApplication")
                .WithTags("Drivers")
                .WithSummary("Submit Driver Application")
                .WithDescription(
                    "Submit a new driver application with personal, vehicle, " +
                    "national ID, and identity/license documents.");

            return app;
        }
    }
}
