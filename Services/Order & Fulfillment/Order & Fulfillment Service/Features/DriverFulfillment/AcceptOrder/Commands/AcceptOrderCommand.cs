using Blocks.Contracts.Common;
using MediatR;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AcceptOrder.Commands;

public sealed record AcceptOrderCommand(Guid DriverId, Guid OrderId) : IRequest<Result>;
