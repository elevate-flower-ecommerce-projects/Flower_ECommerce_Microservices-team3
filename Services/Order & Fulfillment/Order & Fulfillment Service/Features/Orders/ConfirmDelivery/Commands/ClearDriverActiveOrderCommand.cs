using MediatR;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands
{
    public sealed record ClearDriverActiveOrderCommand(Guid DriverId) : IRequest<bool>;
}
