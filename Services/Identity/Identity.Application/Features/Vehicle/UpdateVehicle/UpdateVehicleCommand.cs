using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Common;
using Identity.Domain.Enums;
using MediatR;

using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Vehicle.UpdateVehicle
{
    public sealed record UpdateVehicleCommand(
        Guid DriverId,
        VehicleType VehicleType,
        string VehicleNumber,
        IFormFile? VehicleLicenceFile = null
    ) : IRequest<Result>;
}
