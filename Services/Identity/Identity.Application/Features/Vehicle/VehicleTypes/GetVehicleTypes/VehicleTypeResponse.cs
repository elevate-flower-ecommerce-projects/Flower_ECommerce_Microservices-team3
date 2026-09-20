using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Vehicle.VehicleTypes.GetVehicleTypes
{
    public sealed record VehicleTypeResponse(
        int Id,
        string Name
    );
}
