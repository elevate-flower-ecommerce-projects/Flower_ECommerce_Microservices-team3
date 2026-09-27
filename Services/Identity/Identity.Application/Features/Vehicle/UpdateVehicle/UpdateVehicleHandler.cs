using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Identity.Application.Features.Vehicle.UpdateVehicle;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Features.Drivers.Vehicle.UpdateVehicle;

public sealed class UpdateVehicleHandler(
        IDriverRepository driverRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateVehicleCommand, Result>
{
    public async Task<Result> Handle(
        UpdateVehicleCommand request,
        CancellationToken cancellationToken)
    {
        var driver = await driverRepository.GetByDriverOrUserIdAsync(
            request.DriverId,
            cancellationToken);

        if (driver is null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound"));
        }

        driver.VehicleType = request.VehicleType;
        driver.VehicleNumber = request.VehicleNumber;

        if (request.VehicleLicenceFile is not null && request.VehicleLicenceFile.Length > 0)
        {
            driver.VehicleLicenceImage = await fileStorageService.UploadAsync(
                request.VehicleLicenceFile,
                $"drivers/{driver.Id}/vehicle-licence",
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}