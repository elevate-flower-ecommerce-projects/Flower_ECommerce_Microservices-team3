using Blocks.Contracts.Common;
using DomainError = Blocks.Domain.Errors.Error;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Features.Drivers.Commands.SubmitDriverApplication;

public sealed class SubmitDriverApplicationCommandHandler(
    IUserRepository userRepo,
    IDriverRepository driverRepo,
    IDriverApplicationRepository driverApplicationRepo,
    IFileStorageService fileStorageService,
    IPasswordService passwordService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        SubmitDriverApplicationCommand,
        Result<SubmitDriverApplicationResponse>>
{
    public async Task<Result<SubmitDriverApplicationResponse>> Handle(
        SubmitDriverApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // Check duplicate email
        if (await userRepo.ExistsByEmailAsync(
                request.Email,
                cancellationToken))
        {
            return Result.Failure<SubmitDriverApplicationResponse>(
                DomainError.Conflict("AUTH_EMAIL_EXISTS"));
        }

        // Check duplicate phone
        var phone = request.CountryCode + request.PhoneNumber;

        if (await userRepo.ExistsByPhoneAsync(
                phone,
                cancellationToken))
        {
            return Result.Failure<SubmitDriverApplicationResponse>(
                DomainError.Conflict("AUTH_PHONE_EXISTS"));
        }

        // Check duplicate National ID
        if (await driverRepo.ExistsByNationalIdAsync(
                request.NationalId,
                cancellationToken))
        {
            return Result.Failure<SubmitDriverApplicationResponse>(
                DomainError.Conflict("AUTH_NATIONAL_ID_EXISTS"));
        }

        // Create User
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.SecondName,
            Email = request.Email,
            Phone = phone,
            Gender = request.Gender,
            HashPassword = passwordService.Hash(request.Password)
        };

        userRepo.Add(user);

        // Create Driver
        var driver = new Driver
        {
            UserId = user.Id,
            VehicleType = request.VehicleType,
            VehicleNumber = request.VehicleNumber,
            NationalIdNumber = request.NationalId
        };

        // Upload Vehicle Licence if provided
        if (request.VehicleLicenceFile is not null)
        {
            driver.VehicleLicenceImage =
                await fileStorageService.UploadAsync(
                    request.VehicleLicenceFile,
                    $"drivers/{driver.Id}/vehicle-licence",
                    cancellationToken);
        }

        // Upload National ID if provided
        if (request.IdImage is not null)
        {
            driver.NationalIdImage =
                await fileStorageService.UploadAsync(
                    request.IdImage,
                    $"drivers/{driver.Id}/national-id",
                    cancellationToken);
        }

        driverRepo.Add(driver);

        // Create Driver Application
        var application = new DriverApplication(user.Id);

        driverApplicationRepo.Add(application);

        // Save changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new SubmitDriverApplicationResponse(
            application.Id,
            application.Status);

        return Result.Success(response);
    }
}