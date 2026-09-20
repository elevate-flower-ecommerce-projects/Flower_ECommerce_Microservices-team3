using MediatR;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation.Queries
{
    public sealed record GetActiveOrderForDriverQuery(Guid DriverId) : IRequest<Guid?>;
}
