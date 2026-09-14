using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.OrderHistory.Queries;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.OrderHistory.Queries;

public sealed class GetOrderHistoryQueryHandler(
    IGenericRepository<Order> orderRepository,
    IAddressServiceClient addressServiceClient)
    : IRequestHandler<GetOrderHistoryQuery, Result<AvailableOrderListDto>>
{
    public async Task<Result<AvailableOrderListDto>> Handle(
        GetOrderHistoryQuery request,
        CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var query = orderRepository.GetQueryable()
            .AsNoTracking()
            .Where(o => o.AssignedDriverId == request.DriverId);

        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var rawOrders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                o.Id,
                o.Status,
                o.StoreId,
                o.RecipientName,
                o.City,
                o.Area,
                ItemCount = o.Items.Count,
                o.Total,
                o.CreatedAt,
                o.AssignedAt,
                DeliveredAt = o.Status == OrderStatus.Delivered ? (DateTime?)o.UpdatedAt : null
            })
            .ToListAsync(ct);

        var distinctStoreIds = rawOrders.Select(o => o.StoreId).Distinct().ToList();
        var storeTasks = distinctStoreIds.ToDictionary(
            id => id,
            id => addressServiceClient.GetStoreByIdAsync(id, ct));

        await Task.WhenAll(storeTasks.Values);

        var storeMap = storeTasks.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Result);

        var items = rawOrders.Select(o =>
        {
            var store = storeMap.GetValueOrDefault(o.StoreId);
            return new DriverOrderSummaryDto(
                o.Id,
                o.Status.ToString(),
                new StoreSummaryDto(
                    store?.Name ?? "Unknown Store",
                    store is not null ? $"{store.Lat}, {store.Lng}" : "Store Location"),
                new RecipientSummaryDto(
                    o.RecipientName,
                    o.City,
                    o.Area),
                o.ItemCount,
                o.Total,
                EstimatedDeliveryAt: null,
                DeliveredAt: o.DeliveredAt,
                AssignedAt: o.AssignedAt);
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagination = new PaginationMetadataDto(
            page,
            pageSize,
            totalCount,
            totalPages,
            page < totalPages,
            page > 1);

        return Result.Success(new AvailableOrderListDto(items, pagination));
    }
}
