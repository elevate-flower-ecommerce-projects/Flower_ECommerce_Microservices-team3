using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands
{
    public class ConfirmOrderDeliveryCommandHandler(
        IGenericRepository<Order> orderRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ConfirmOrderDeliveryCommand, Result<string>>
    {
        private static readonly OrderStatus[] ConfirmableStatuses =
        [
            OrderStatus.OutForDelivery,
            OrderStatus.AwaitingDeliveryConfirmation
        ];

        public async Task<Result<string>> Handle(
            ConfirmOrderDeliveryCommand request,
            CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetQueryable()
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                return Result.Failure<string>(Error.NotFound("Order not found."));
            }

            if (order.CustomerId != request.CustomerId)
            {
                return Result.Failure<string>(Error.Forbidden("You are not authorized to confirm this order."));
            }

            if (!ConfirmableStatuses.Contains(order.Status))
            {
                return Result.Failure<string>(
                    Error.Validation($"Order cannot be confirmed in its current status ({order.Status})."));
            }

            order.Status = OrderStatus.Delivered;
            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Order delivery confirmed successfully.");
        }
    }
}
