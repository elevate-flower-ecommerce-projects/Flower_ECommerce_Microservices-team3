using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.Commands;

public sealed record UpdateOrderStatusCommand(
    Guid DriverId,
    Guid OrderId,
    DriverStatusUpdate NewStatus)
    : IRequest<Result>;
