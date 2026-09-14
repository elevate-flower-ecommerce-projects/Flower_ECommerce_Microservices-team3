using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.Queries;

public sealed record GetAvailableOrdersQuery(int Page = 1, int PageSize = 10)
    : IRequest<Result<AvailableOrderListDto>>;
