using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.Register.CustomerRegisteration.Commands;
using Identity.Application.Features.Register.CustomerRegisteration.ViewModels;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.Register.CustomerRegisteration.Commands.CommandHandlers
{
    public class RegisterCommandHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<Customer> customerRepository,
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
                var validationError = await ValidateUserDoesNotExist(request.Email, request.PhoneNumber);
                if (validationError != null) return Result.Failure<RegisterResponseVm>(validationError);

                var user = await CreateUserAsync(request);
                await CreateCustomerAsync(user.Id);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return Result.Success(new RegisterResponseVm(user.Id, true));
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        private async Task<Error?> ValidateUserDoesNotExist(string email, string phoneNumber)
        {
            var existingUserByEmail = await userRepository.FindAsync(u => u.Email == email.ToLowerInvariant());
            if (existingUserByEmail.Any())
                return Error.Conflict("Email already registered.");

            var existingUserByPhone = await userRepository.FindAsync(u => u.Phone == phoneNumber);
            if (existingUserByPhone.Any())
                return Error.Conflict("Phone number already registered.");

            return null;
        }

        private async Task<User> CreateUserAsync(RegisterCommand request)
        {
            var user = new User
            {
                Id = Guid.CreateVersion7(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.Trim().ToLowerInvariant(),
                HashPassword = passwordService.Hash(request.Password),
                Phone = request.PhoneNumber,
                Gender = request.Gender,
                Role = UserRole.Customer
            };

            await userRepository.AddAsync(user);
            return user;
        }

        private async Task CreateCustomerAsync(Guid userId)
        {
            var customer = new Customer
            {
                Id = Guid.CreateVersion7(),
                UserId = userId
            };

            await customerRepository.AddAsync(customer);
        }
    }
}