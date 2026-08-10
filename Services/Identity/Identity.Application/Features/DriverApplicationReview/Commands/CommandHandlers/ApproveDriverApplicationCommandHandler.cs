using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class ApproveDriverApplicationCommandHandler(IGenericRepository<Identity.Domain.Entities.DriverApplication> driverAppRepository)
        : IRequestHandler<ApproveDriverApplicationCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(ApproveDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await driverAppRepository.GetByIdAsync(request.ApplicationId);

            if (application is null)
                return Result.Failure<bool>(Error.NotFound("Application not found."));

            if (application.Status != DriverApplicationStatus.Pending)
                return Result.Failure<bool>(Error.Conflict("Application is already decided."));

            application.Approve(request.AdminId);
            driverAppRepository.Update(application);

            return Result.Success(true);
        }
    }
}
