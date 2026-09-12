using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.OrderDetail.Queries;

public sealed record GetOrderDetailQuery(Guid DriverId, Guid OrderId)
    : IRequest<Result<DriverOrderDetailDto>>;
