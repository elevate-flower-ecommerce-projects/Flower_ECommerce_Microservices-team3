using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class DeleteUserCommandHandler(IGenericRepository<User> userRepository)
        : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserId);

            if (user is not null)
            {
                userRepository.Delete(user);
            }

            return Result.Success(true);
        }
    }
}