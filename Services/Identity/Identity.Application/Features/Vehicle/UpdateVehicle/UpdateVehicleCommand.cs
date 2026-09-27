using Blocks.Contracts.Common;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Vehicle.UpdateVehicle;

public sealed record UpdateVehicleCommand(
    Guid DriverId,
    VehicleType VehicleType,
    string VehicleNumber,
    IFormFile LicenseDocument) 
                                      : IRequest<Result>;

