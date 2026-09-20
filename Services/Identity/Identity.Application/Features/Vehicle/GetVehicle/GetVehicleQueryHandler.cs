using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Identity.Application.Features.Vehicle.GetVehicle;
using Identity.Application.Interfaces;
using MediatR;

public sealed class GetVehicleHandler(IDriverRepository driverRepository)
    : IRequestHandler<GetVehicleQuery, Result<GetVehicleResponse>>
{

    public async Task<Result<GetVehicleResponse>> Handle(
        GetVehicleQuery request,
        CancellationToken cancellationToken)
    {
        var driver = await driverRepository
            .GetByIdAsNoTrackingAsync(
                request.DriverId,
                cancellationToken);

        if (driver is null)
        {
            return Result.Failure<GetVehicleResponse>(
                Error.NotFound("Driver.NotFound"));
        }

        return Result.Success(
            new GetVehicleResponse(
                driver.VehicleType,
                driver.VehicleNumber));
    }
}