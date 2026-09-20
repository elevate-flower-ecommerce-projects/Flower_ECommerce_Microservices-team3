using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Common;
using MediatR;

namespace Identity.Application.Features.Vehicle.GetVehicle
{
    public sealed record GetVehicleQuery(
        Guid DriverId
    ) : IRequest<Result<GetVehicleResponse>>;
}
