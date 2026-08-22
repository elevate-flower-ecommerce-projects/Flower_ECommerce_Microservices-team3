using Blocks.Contracts.Common;
using Identity.Application.Features.DriverApplicationReview.Commands;
using Identity.Application.Features.DriverApplicationReview.DTOs;
using Identity.Application.Features.DriverApplicationReview.Queries;
using Identity.Application.Interfaces;
using Identity.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Orchestrators.OrchestratorHandlers
{
    public class ApproveDriverApplicationOrchestratorHandler(IMediator mediator, IUnitOfWork unitOfWork)
        : IRequestHandler<ApproveDriverApplicationOrchestrator, Result<bool>>
    {
        public async Task<Result<bool>> Handle(ApproveDriverApplicationOrchestrator request, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var appResult = await mediator.Send(new GetDriverApplicationByIdQuery(request.ApplicationId), cancellationToken);
                if (!appResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<bool>(appResult.Error);
                }

                var approveResult = await mediator.Send(new ApproveDriverApplicationCommand(request.ApplicationId, request.AdminId), cancellationToken);
                if (!approveResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<bool>(approveResult.Error);
                }

                var activateResult = await mediator.Send(new ActivateUserCommand(appResult.Value.UserId), cancellationToken);
                if (!activateResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<bool>(activateResult.Error);
                }

                var driverDto = new CreateDriverDto(
                    appResult.Value.UserId,
                    request.ApplicationId,
                    Enum.Parse<VehicleType>(appResult.Value.VehicleType),
                    appResult.Value.VehicleNumber,
                    appResult.Value.VehicleLicenceImage,
                    appResult.Value.NationalIdNumber,
                    appResult.Value.NationalIdImage
                );

                var createDriverResult = await mediator.Send(new CreateDriverCommand(driverDto), cancellationToken);
                if (!createDriverResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<bool>(createDriverResult.Error);
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                await mediator.Send(new PublishDriverApprovedEventCommand(appResult.Value.UserId, appResult.Value.Email), cancellationToken);

                return Result.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
