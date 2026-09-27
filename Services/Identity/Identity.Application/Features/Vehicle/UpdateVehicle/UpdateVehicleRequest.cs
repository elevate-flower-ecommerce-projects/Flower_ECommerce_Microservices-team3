using System;
using System.Collections.Generic;
using System.Text;
using Identity.Domain.Enums;

using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Vehicle.UpdateVehicle
{
    public sealed record UpdateVehicleRequest(
        VehicleType VehicleType,
        string VehicleNumber,
        IFormFile? VehicleLicenceFile = null
    );
}
