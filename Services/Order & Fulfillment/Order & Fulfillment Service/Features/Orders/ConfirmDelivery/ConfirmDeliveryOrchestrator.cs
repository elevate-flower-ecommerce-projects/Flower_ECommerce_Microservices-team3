using Blocks.Contracts.Common;
using MediatR;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery
{
    public sealed record ConfirmDeliveryOrchestrator(
     Guid OrderId,
     Guid CustomerId
 ) : IRequest<Result<string>>;
}
