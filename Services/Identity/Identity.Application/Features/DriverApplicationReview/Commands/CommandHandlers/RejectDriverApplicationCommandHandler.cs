using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class RejectDriverApplicationCommandHandler(IGenericRepository<DriverApplication> driverAppRepository)
        : IRequestHandler<RejectDriverApplicationCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RejectDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await driverAppRepository.GetByIdAsync(request.ApplicationId);

            if (application is null)
                return Result.Failure<bool>(Error.NotFound("Application not found."));

            if (application.Status != DriverApplicationStatus.Pending)
                return Result.Failure<bool>(Error.Conflict("Application is already decided."));

            application.Reject(request.Reason, request.AdminId);
            driverAppRepository.Update(application);

            return Result.Success(true);
        }
    }
}