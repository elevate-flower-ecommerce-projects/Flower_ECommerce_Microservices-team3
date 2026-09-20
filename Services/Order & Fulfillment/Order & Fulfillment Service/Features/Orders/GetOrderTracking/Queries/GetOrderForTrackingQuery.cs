using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Queries
{
    public sealed record GetOrderForTrackingQuery(
     Guid OrderId,
     Guid CustomerId
 ) : IRequest<Result<OrderTrackingProjection>>;
}
