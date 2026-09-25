using Blocks.Contracts.Common;
using DomainError = Blocks.Domain.Errors.Error;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
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

        // Create User (Pending Driver)
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.SecondName,
            Email = request.Email,
            Phone = phone,
            Gender = request.Gender,
            Role = UserRole.Driver,
            IsActive = false,
            HashPassword = passwordService.Hash(request.Password)
        };

        userRepo.Add(user);

        // Upload Vehicle Licence if provided
        string vehicleLicenceUrl = string.Empty;
        if (request.VehicleLicenceFile is not null)
        {
            vehicleLicenceUrl = await fileStorageService.UploadAsync(
                request.VehicleLicenceFile,
                $"drivers/{user.Id}/vehicle-licence",
                cancellationToken);
        }

        // Upload National ID if provided
        string nationalIdUrl = string.Empty;
        if (request.IdImage is not null)
        {
            nationalIdUrl = await fileStorageService.UploadAsync(
                request.IdImage,
                $"drivers/{user.Id}/national-id",
                cancellationToken);
        }

        // Create Driver Application
        var application = new Identity.Domain.Entities.DriverApplication
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            VehicleType = request.VehicleType,
            VehicleNumber = request.VehicleNumber,
            VehicleLicenceImage = vehicleLicenceUrl,
            NationalIdNumber = request.NationalId,
            NationalIdImage = nationalIdUrl
        };

        driverApplicationRepo.Add(application);

        // Save changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new SubmitDriverApplicationResponse(
            application.Id,
            application.Status);

        return Result.Success(response);
    }
}