using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Drivers.ReportLocation.Commands;
using Order___Fulfillment_Service.Features.Drivers.ReportLocation.Queries;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation
{
    public class ReportDriverLocationOrchestratorHandler(IMediator mediator)
     : IRequestHandler<ReportDriverLocationOrchestrator, Result<string?>>
    {
        public async Task<Result<string?>> Handle(
            ReportDriverLocationOrchestrator request,
            CancellationToken cancellationToken)
        {
            
            var activeOrderId = await mediator.Send(
                new GetActiveOrderForDriverQuery(request.DriverId),
                cancellationToken);
            if (activeOrderId == null)
            {
                return Result.Success<string?>(null);
            }
            
            await mediator.Send(
                new UpsertDriverLocationCommand(
                    request.DriverId,
                    activeOrderId.Value,
                    request.Lat,
                    request.Lng,
                    request.RecordedAt),
                cancellationToken);
            return Result.Success<string?>("Location recorded successfully");
        }
    }
}
