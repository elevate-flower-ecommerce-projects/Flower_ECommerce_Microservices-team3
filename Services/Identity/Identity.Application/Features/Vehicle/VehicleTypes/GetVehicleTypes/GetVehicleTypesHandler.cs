using Blocks.Contracts.Common;
using Identity.Application.Features.VehicleTypes.GetVehicleTypes;
using Identity.Domain.Enums;
using MediatR;

namespace Identity.Application.Features.Vehicle.VehicleTypes.GetVehicleTypes;

public sealed class GetVehicleTypesHandler
    : IRequestHandler<GetVehicleTypesQuery, Result<List<VehicleTypeResponse>>>
{
    public Task<Result<List<VehicleTypeResponse>>> Handle(
        GetVehicleTypesQuery request,
        CancellationToken cancellationToken)
    {
        var vehicleTypes = Enum
            .GetValues<VehicleType>()
            .Select(vehicleType => new VehicleTypeResponse(
                (int)vehicleType,
                vehicleType.ToString()))
            .ToList();

        return Task.FromResult(Result.Success(vehicleTypes));
    }
}