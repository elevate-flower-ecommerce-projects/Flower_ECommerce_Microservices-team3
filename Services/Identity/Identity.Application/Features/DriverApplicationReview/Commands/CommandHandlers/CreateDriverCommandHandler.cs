using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Identity.Application.Features.DriverApplicationReview.Commands;
using Identity.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class CreateDriverCommandHandler(IGenericRepository<Driver> driverRepository)
        : IRequestHandler<CreateDriverCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = new Driver
            {
                UserId = request.Data.UserId,
                DriverApplicationId = request.Data.DriverApplicationId,
                VehicleType = request.Data.VehicleType,
                VehicleNumber = request.Data.VehicleNumber,
                VehicleLicenceImage = request.Data.VehicleLicenceImage,
                NationalIdNumber = request.Data.NationalIdNumber,
                NationalIdImage = request.Data.NationalIdImage,
                CreatedAt = DateTime.UtcNow
            };

            driver.Activate();
            await driverRepository.AddAsync(driver);

            return Result.Success(true);
        }
    }
}