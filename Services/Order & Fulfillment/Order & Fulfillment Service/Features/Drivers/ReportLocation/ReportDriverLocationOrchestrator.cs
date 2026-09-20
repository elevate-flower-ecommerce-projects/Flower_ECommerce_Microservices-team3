using Blocks.Contracts.Common;
using MediatR;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation
{
    public sealed record ReportDriverLocationOrchestrator(
    Guid DriverId,
    double Lat,
    double Lng,
    DateTime RecordedAt
) : IRequest<Result<string?>>;
}
