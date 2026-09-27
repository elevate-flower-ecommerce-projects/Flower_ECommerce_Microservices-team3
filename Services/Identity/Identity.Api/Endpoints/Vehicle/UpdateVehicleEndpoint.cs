using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Domain.Errors;
using Identity.Application.Features.Vehicle.UpdateVehicle;
using MediatR;

namespace Identity.Api.Features.Vehicle
{
    public static class UpdateVehicleEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateVehicleEndpoint(
            this IEndpointRouteBuilder app)
        {
            var handler = async (
                HttpContext httpContext,
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var driverId))
                {
                    return Results.Json(
                        ApiResponse<object>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                Identity.Domain.Enums.VehicleType vehicleType = Identity.Domain.Enums.VehicleType.Car;
                string vehicleNumber = string.Empty;
                IFormFile? vehicleLicenceFile = null;

                if (httpContext.Request.HasFormContentType)
                {
                    var form = await httpContext.Request.ReadFormAsync(cancellationToken);

                    var typeStr = form["vehicleType"].FirstOrDefault() ?? form["VehicleType"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(typeStr))
                    {
                        if (int.TryParse(typeStr, out var vInt) && Enum.IsDefined(typeof(Identity.Domain.Enums.VehicleType), vInt))
                        {
                            vehicleType = (Identity.Domain.Enums.VehicleType)vInt;
                        }
                        else if (Enum.TryParse<Identity.Domain.Enums.VehicleType>(typeStr, true, out var vEnum))
                        {
                            vehicleType = vEnum;
                        }
                    }

                    vehicleNumber = form["vehicleNumber"].FirstOrDefault() ?? form["VehicleNumber"].FirstOrDefault() ?? string.Empty;
                    vehicleLicenceFile = form.Files.GetFile("vehicleLicenceFile")
                                      ?? form.Files.GetFile("VehicleLicenceFile")
                                      ?? (form.Files.Count > 0 ? form.Files[0] : null);
                }
                else if (httpContext.Request.HasJsonContentType())
                {
                    var jsonBody = await httpContext.Request.ReadFromJsonAsync<UpdateVehicleRequest>(cancellationToken: cancellationToken);
                    if (jsonBody is not null)
                    {
                        vehicleType = jsonBody.VehicleType;
                        vehicleNumber = jsonBody.VehicleNumber;
                    }
                }

                var command = new UpdateVehicleCommand(
                    driverId,
                    vehicleType,
                    vehicleNumber,
                    vehicleLicenceFile);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<bool>.Ok(true, "Vehicle updated successfully."));
                }

                return Results.Json(
                    ApiResponse<bool>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            };

            app.MapPatch("/api/v1/drivers/me/vehicle", handler)
                .WithName("UpdateDriverVehicle")
                .WithTags("Vehicle")
                .WithSummary("Update Driver Vehicle (Multipart Form / JSON)")
                .WithDescription("Updates the vehicle type, plate number, and optional vehicle license file for the authenticated driver.")
                .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<bool>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
                .RequireAuthorization()
                .DisableAntiforgery();

            app.MapPatch("/api/drivers/me/vehicle", handler).ExcludeFromDescription().RequireAuthorization().DisableAntiforgery();
            app.MapPatch("/drivers/me/vehicle", handler).ExcludeFromDescription().RequireAuthorization().DisableAntiforgery();

            return app;
        }
    }
}

