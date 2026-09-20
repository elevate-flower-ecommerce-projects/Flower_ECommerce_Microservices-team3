using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking
{
    public sealed record GetOrderTrackingOrchestrator(
    Guid CustomerId,
    Guid OrderId
) : IRequest<Result<OrderTrackingDataDto>>;
}
