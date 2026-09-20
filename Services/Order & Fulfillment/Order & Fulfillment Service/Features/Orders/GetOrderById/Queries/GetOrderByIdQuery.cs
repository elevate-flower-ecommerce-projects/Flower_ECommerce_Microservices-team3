using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderById.Queries
{
    public sealed record GetOrderByIdQuery(
    Guid CustomerId,
    Guid OrderId) : IRequest<Result<OrderDetailDto>>;
}
