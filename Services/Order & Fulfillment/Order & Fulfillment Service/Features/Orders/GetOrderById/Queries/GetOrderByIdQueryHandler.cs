using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs;
using Order___Fulfillment_Service.Persistence;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderById.Queries
{
    public sealed class GetOrderByIdQueryHandler(
        IGenericRepository<Order> orderRepository,
        IPaymentServiceClient paymentServiceClient,
        ICartServiceClient cartService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
    {
        public async Task<Result<OrderDetailDto>> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetQueryable()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId
                                       && o.CustomerId == request.CustomerId 
                                       && o.DeletedAt == null,
                                     cancellationToken);

            if (order is null)
            {
                return Result.Failure<OrderDetailDto>(
                    Error.NotFound("Order not found."));
            }

            if (order.Status == OrderStatus.PendingPayment && order.PaymentMethod == PaymentMethod.Card)
            {
                var paymentStatus = await paymentServiceClient.GetPaymentStatusAsync(order.Id, ct: cancellationToken);
                if (paymentStatus is not null &&
                    (string.Equals(paymentStatus.PaymentStatus, "Succeeded", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(paymentStatus.OrderStatus, "Preparing", StringComparison.OrdinalIgnoreCase)))
                {
                    order.Status = OrderStatus.Preparing;
                    order.UpdatedAt = DateTime.UtcNow;
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    var cartId = order.CartId ?? Guid.Empty;
                    await cartService.ClearCartAsync(cartId, order.CustomerId, ct: cancellationToken);
                }
            }

            var dto = new OrderDetailDto(
                order.Id,
                order.Status,
                order.PaymentMethod,
                order.PaymentProvider != null ? order.PaymentProvider.ToString() : null,
                order.Items.OrderBy(i => i.ProductName)
                       .Select(i => new OrderItemDto(
                           i.ProductId,
                           i.ProductName,
                           i.Quantity,
                           i.UnitPrice))
                       .ToList(),
                new OrderAddressDto(
                    order.RecipientName,
                    order.RecipientPhone,
                    order.AddressLine,
                    order.City,
                    order.Area),
                order.Subtotal,
                order.DeliveryFee,
                order.Total,
                order.IsGift,
                order.GiftRecipientName,
                order.GiftRecipientPhone,
                order.EstimatedDeliveryAt,
                order.CreatedAt);

            return Result.Success(dto);
        }
    }
}
