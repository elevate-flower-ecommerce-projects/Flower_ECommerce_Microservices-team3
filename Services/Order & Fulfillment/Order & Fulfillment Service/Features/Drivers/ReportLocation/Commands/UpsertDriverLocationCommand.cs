using MediatR;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation.Commands
{
    public sealed record UpsertDriverLocationCommand(
        Guid DriverId,
        Guid ActiveOrderId,
        double Lat,
        double Lng,
        DateTime RecordedAt
    ) : IRequest<bool>;
}
