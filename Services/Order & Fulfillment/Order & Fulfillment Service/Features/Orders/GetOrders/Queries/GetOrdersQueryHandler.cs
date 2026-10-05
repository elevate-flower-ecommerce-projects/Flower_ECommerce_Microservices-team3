using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs;
using Order___Fulfillment_Service.Persistence;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders.Queries
{
    public sealed class GetOrdersQueryHandler(
        IGenericRepository<Order> orderRepository,
        IPaymentServiceClient paymentServiceClient,
        ICartServiceClient cartService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<GetOrdersQuery, Result<PagedResult<OrderListItemDto>>>
    {
        public async Task<Result<PagedResult<OrderListItemDto>>> Handle(
            GetOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var query = orderRepository.GetQueryable()
                .Where(o => o.CustomerId == request.CustomerId && o.DeletedAt == null);

            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(o => o.Items)
                .ToListAsync(cancellationToken);

            foreach (var order in orders)
            {
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
            }

            var items = orders.Select(o => new OrderListItemDto(
                o.Id,
                o.Status,
                o.Items.Count,
                o.Items.OrderBy(i => i.ProductName)
                       .Select(i => i.ThumbnailUrl)
                       .FirstOrDefault(),
                o.Total,
                o.EstimatedDeliveryAt,
                o.CreatedAt)).ToList();

            var pagedResult = PagedResult<OrderListItemDto>.Create(
                items,
                totalCount,
                new PaginationParams
                {
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                });

            return Result.Success(pagedResult);
        }
    }
}
