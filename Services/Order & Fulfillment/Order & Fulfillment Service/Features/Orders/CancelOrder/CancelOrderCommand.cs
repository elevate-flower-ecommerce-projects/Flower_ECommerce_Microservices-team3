using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.Orders.CancelOrder;

public sealed record CancelOrderRequest(string? Reason);

public sealed record CancelOrderCommand(
    Guid OrderId,
    Guid CustomerId,
    string? Reason
) : IRequest<Result<string>>;

public sealed class CancelOrderCommandHandler(
    IGenericRepository<Order> orderRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CancelOrderCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetQueryable()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<string>(Error.NotFound("Order not found."));
        }

        if (order.CustomerId != request.CustomerId)
        {
            return Result.Failure<string>(Error.Forbidden("You are not authorized to cancel this order."));
        }

        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.OutForDelivery or OrderStatus.PickedUp)
        {
            return Result.Failure<string>(
                Error.Validation($"Order cannot be cancelled in its current status ({order.Status})."));
        }

        order.Status = OrderStatus.Cancelled;
        order.CancellationReason = string.IsNullOrWhiteSpace(request.Reason)
            ? "Cancelled by customer"
            : request.Reason.Trim();
        order.UpdatedAt = DateTime.UtcNow;

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Order cancelled successfully.");
    }
}
