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
            
            // Always save location, even if no active order exists
            await mediator.Send(
                new UpsertDriverLocationCommand(
                    request.DriverId,
                    activeOrderId,
                    request.Lat,
                    request.Lng,
                    request.RecordedAt),
                cancellationToken);

            return activeOrderId != null
                ? Result.Success<string?>("Location recorded successfully")
                : Result.Success<string?>("Location recorded (no active order)");
        }
    }
}
