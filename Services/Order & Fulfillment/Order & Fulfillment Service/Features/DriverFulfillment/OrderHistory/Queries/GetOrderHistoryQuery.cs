using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.OrderHistory.Queries;

public sealed record GetOrderHistoryQuery(
    Guid DriverId,
    int Page = 1,
    int PageSize = 10,
    OrderStatus? Status = null)
    : IRequest<Result<AvailableOrderListDto>>;
