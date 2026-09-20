using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.ActiveOrder.Queries;

public sealed record GetActiveOrderQuery(Guid DriverId) : IRequest<Result<DriverOrderDetailDto?>>;
