using System;
using System.Collections.Generic;
using System.Text;
using Identity.Domain.Enums;

namespace Identity.Application.Features.Vehicle.GetVehicle
{
    public sealed record GetVehicleResponse(
        VehicleType VehicleType,
        string VehicleNumber
    );
}
