using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation.Queries
{
    public class GetActiveOrderForDriverQueryHandler(
    IGenericRepository<Order> orderRepository)
    : IRequestHandler<GetActiveOrderForDriverQuery, Guid?>
    {
        private static readonly HashSet<OrderStatus> LiveStatuses = new()
    {
        OrderStatus.Placed,
        OrderStatus.Preparing,
        OrderStatus.PickedUp,
        OrderStatus.OutForDelivery,
        OrderStatus.AwaitingDeliveryConfirmation
    };
        public async Task<Guid?> Handle(
            GetActiveOrderForDriverQuery request,
            CancellationToken cancellationToken)
        {
            var activeOrder = await orderRepository.GetQueryable()
                .AsNoTracking()
                .Where(o => o.AssignedDriverId == request.DriverId && LiveStatuses.Contains(o.Status))
                .Select(o => (Guid?)o.Id)
                .FirstOrDefaultAsync(cancellationToken);
            return activeOrder;
        }
    }

}
