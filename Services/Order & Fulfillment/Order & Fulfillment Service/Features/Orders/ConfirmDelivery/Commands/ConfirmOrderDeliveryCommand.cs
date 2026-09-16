using Blocks.Contracts.Common;
using MediatR;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands
{
    public sealed record ConfirmOrderDeliveryCommand(
    Guid OrderId,
    Guid CustomerId
) : IRequest<Result<Guid?>>;
}
