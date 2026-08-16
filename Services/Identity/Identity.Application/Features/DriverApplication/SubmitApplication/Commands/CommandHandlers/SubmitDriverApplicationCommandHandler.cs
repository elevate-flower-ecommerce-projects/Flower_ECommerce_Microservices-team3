using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.DriverApplication.SubmitApplication.Commands;
using Identity.Application.Features.DriverApplication.SubmitApplication.ViewModels;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;

namespace Identity.Application.Features.DriverApplication.SubmitApplication.Commands.CommandHandlers;

public class SubmitDriverApplicationCommandHandler(
    IGenericRepository<User> userRepository,
    IGenericRepository<Domain.Entities.DriverApplication> driverAppRepository,
    IPasswordService passwordService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitDriverApplicationCommand, Result<SubmitDriverApplicationResponseVm>>
{
    public async Task<Result<SubmitDriverApplicationResponseVm>> Handle(
        SubmitDriverApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var emailLower = dto.Email.Trim().ToLowerInvariant();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingUserByEmail = await userRepository.FindAsync(u => u.Email == emailLower);
            if (existingUserByEmail.Any())
            {
                return Result.Failure<SubmitDriverApplicationResponseVm>(
                    Error.Conflict("Email already registered."));
            }

            var existingUserByPhone = await userRepository.FindAsync(u => u.Phone == dto.PhoneNumber);
            if (existingUserByPhone.Any())
            {
                return Result.Failure<SubmitDriverApplicationResponseVm>(
                    Error.Conflict("Phone number already registered."));
            }

            var existingAppByNid = await driverAppRepository.FindAsync(da => da.NationalIdNumber == dto.NationalIdNumber);
            if (existingAppByNid.Any())
            {
                return Result.Failure<SubmitDriverApplicationResponseVm>(
                    Error.Conflict("National ID is already registered."));
            }

            var nameParts = dto.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.Length > 0 ? nameParts[0] : dto.FullName;
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var passwordHash = passwordService.Hash(dto.Password);

            var user = new User
            {
                Id = Guid.CreateVersion7(),
                FirstName = firstName,
                LastName = lastName,
                Email = emailLower,
                HashPassword = passwordHash,
                Phone = dto.PhoneNumber,
                Role = UserRole.Driver
            };

            await userRepository.AddAsync(user);

            var driverApp = new Domain.Entities.DriverApplication
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                VehicleType = dto.VehicleType,
                VehicleNumber = dto.VehicleNumber,
                VehicleLicenceImage = dto.VehicleLicenceImage,
                NationalIdNumber = dto.NationalIdNumber,
                NationalIdImage = dto.NationalIdImage,
                CreatedAt = DateTime.UtcNow
            };

            await driverAppRepository.AddAsync(driverApp);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(new SubmitDriverApplicationResponseVm(
                driverApp.Id,
                user.Id,
                driverApp.Status,
                "Driver application submitted successfully."
            ));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
