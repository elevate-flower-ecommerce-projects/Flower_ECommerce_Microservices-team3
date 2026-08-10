using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class ActivateUserCommandHandler(IGenericRepository<User> userRepository)
    : IRequestHandler<ActivateUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                return Result.Failure<bool>(Error.NotFound("User not found."));

            user.Activate();
            userRepository.Update(user);

            return Result.Success(true);
        }
    }
}
