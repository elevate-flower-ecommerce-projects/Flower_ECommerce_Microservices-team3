using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.Register.ViewModels;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Register.Commands.CommandHandlers
{
    public class RegisterCommandHandler(
        IGenericRepository<User> userRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RegisterCommand, Result<RegisterResponseVm>>
    {
        public async Task<Result<RegisterResponseVm>> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var existingUserByEmail = await userRepository.FindAsync(u => u.Email == request.Email.ToLowerInvariant());
                if (existingUserByEmail.Any())
                {
                    return Result.Failure<RegisterResponseVm>(
                        Error.Conflict("Email already registered."));
                }

                var existingUserByPhone = await userRepository.FindAsync(u => u.Phone == request.PhoneNumber);
                if (existingUserByPhone.Any())
                {
                    return Result.Failure<RegisterResponseVm>(
                        Error.Conflict("Phone number already registered."));
                }

                var passwordHash = passwordService.Hash(request.Password);

                var user = new User
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email.Trim().ToLowerInvariant(),
                    HashPassword = passwordHash,
                    Phone = request.PhoneNumber,
                    Gender = request.Gender,
                    Role = UserRole.Customer
                };

                await userRepository.AddAsync(user);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success(new RegisterResponseVm(user.Id, true));
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
