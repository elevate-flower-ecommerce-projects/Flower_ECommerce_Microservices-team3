using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders.Queries
{
    public sealed class GetOrdersQueryHandler(
    IGenericRepository<Order> orderRepository)
    : IRequestHandler<GetOrdersQuery, Result<PagedResult<OrderListItemDto>>>
    {
        public async Task<Result<PagedResult<OrderListItemDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)

        {
            var query = orderRepository.GetQueryable()
           .AsNoTracking()
           .Where(o => o.CustomerId == request.CustomerId && o.DeletedAt == null);
            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
           .OrderByDescending(o => o.CreatedAt)
           .ThenByDescending(o => o.Id)
           .Skip((request.Page - 1) * request.PageSize)
           .Take(request.PageSize)
           .Select(o => new OrderListItemDto(
               o.Id,
               o.Status,
               o.Items.Count, 
               o.Items.OrderBy(i => i.ProductName)
                      .Select(i => i.ThumbnailUrl)
                      .FirstOrDefault(), 
               o.Total,
               o.EstimatedDeliveryAt,
               o.CreatedAt))
           .ToListAsync(cancellationToken);

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
