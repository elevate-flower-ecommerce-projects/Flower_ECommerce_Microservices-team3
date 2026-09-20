using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Commands
{
    public sealed record SaveOrderDriverSnapshotCommand(
    Guid OrderId,
    string DriverName,
    string DriverPhone,
    string? DriverPhotoUrl
) : IRequest<Result<DriverSnapshotDto?>>;
}
