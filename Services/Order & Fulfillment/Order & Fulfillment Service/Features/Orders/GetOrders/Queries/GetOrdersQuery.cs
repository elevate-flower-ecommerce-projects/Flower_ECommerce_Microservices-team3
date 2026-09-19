using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using MediatR;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders.Queries
{
    public sealed record GetOrdersQuery(
     Guid CustomerId,
     int Page = 1,
     int PageSize = 10,
     OrderStatus? Status = null) : IRequest<Result<PagedResult<OrderListItemDto>>>;
}
