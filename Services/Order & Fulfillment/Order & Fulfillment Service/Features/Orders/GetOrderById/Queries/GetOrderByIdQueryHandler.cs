using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderById.Queries
{
    public sealed class GetOrderByIdQueryHandler(
        IGenericRepository<Order> orderRepository)
        : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
    {
        public async Task<Result<OrderDetailDto>> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetQueryable()
                .AsNoTracking()
                .Where(o => o.Id == request.OrderId
                         && o.CustomerId == request.CustomerId 
                         && o.DeletedAt == null)
                .Select(o => new OrderDetailDto(
                    o.Id,
                    o.Status,
                    o.PaymentMethod,
                    o.PaymentProvider != null ? o.PaymentProvider.ToString() : null,
                    o.Items.OrderBy(i => i.ProductName)
                           .Select(i => new OrderItemDto(
                               i.ProductId,
                               i.ProductName,
                               i.Quantity,
                               i.UnitPrice))
                           .ToList(),
                    new OrderAddressDto(
                        o.RecipientName,
                        o.RecipientPhone,
                        o.AddressLine,
                        o.City,
                        o.Area),
                    o.Subtotal,
                    o.DeliveryFee,
                    o.Total,
                    o.IsGift,
                    o.GiftRecipientName,
                    o.GiftRecipientPhone,
                    o.EstimatedDeliveryAt,
                    o.CreatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
            {
                return Result.Failure<OrderDetailDto>(
                    Error.NotFound("Order not found."));
            }

            return Result.Success(order);
        }
    }
}
