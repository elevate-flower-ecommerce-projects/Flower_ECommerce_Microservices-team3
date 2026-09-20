using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Identity.Application.Features.Vehicle.UpdateVehicle;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Features.Drivers.Vehicle.UpdateVehicle;

public sealed class UpdateVehicleHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateVehicleCommand, Result>
{
    public async Task<Result> Handle(
        UpdateVehicleCommand request,
        CancellationToken cancellationToken)
    {
        var driver = await driverRepository.GetByIdAsync(
            request.DriverId,
            cancellationToken);

        if (driver is null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound"));
        }

        driver.VehicleType = request.VehicleType;
        driver.VehicleNumber = request.VehicleNumber;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}