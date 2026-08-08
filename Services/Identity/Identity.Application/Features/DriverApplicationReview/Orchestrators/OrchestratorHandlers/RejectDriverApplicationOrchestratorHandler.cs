using Blocks.Contracts.Common;
using Identity.Application.Features.DriverApplicationReview.Commands;
using Identity.Application.Features.DriverApplicationReview.Queries;
using Identity.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Orchestrators.OrchestratorHandlers
{
    public class RejectDriverApplicationOrchestratorHandler(IMediator mediator, IUnitOfWork unitOfWork)
        : IRequestHandler<RejectDriverApplicationOrchestrator, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RejectDriverApplicationOrchestrator request, CancellationToken cancellationToken)
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

                var rejectResult = await mediator.Send(new RejectDriverApplicationCommand(request.ApplicationId, request.AdminId, request.Reason), cancellationToken);
                if (!rejectResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<bool>(rejectResult.Error);
                }

                var deleteUserResult = await mediator.Send(new DeleteUserCommand(appResult.Value.UserId), cancellationToken);
                if (!deleteUserResult.IsSuccess)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<bool>(deleteUserResult.Error);
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                await mediator.Send(new PublishDriverRejectedEventCommand(appResult.Value.Email, request.Reason), cancellationToken);

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
