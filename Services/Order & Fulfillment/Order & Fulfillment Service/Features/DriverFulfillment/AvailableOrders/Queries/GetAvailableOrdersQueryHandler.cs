using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.Queries;

public sealed class GetAvailableOrdersQueryHandler(
    IGenericRepository<Order> orderRepository,
    IAddressServiceClient addressServiceClient)
    : IRequestHandler<GetAvailableOrdersQuery, Result<AvailableOrderListDto>>
{
    public async Task<Result<AvailableOrderListDto>> Handle(
        GetAvailableOrdersQuery request,
        CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var query = orderRepository.GetQueryable()
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Preparing && o.AssignedDriverId == null);

        var totalCount = await query.CountAsync(ct);

        // FIFO — oldest first; projection without loading full entity
        var rawOrders = await query
            .OrderBy(o => o.CreatedAt)
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
                o.EstimatedDeliveryAt,
                o.CreatedAt,
                o.AssignedAt
            })
            .ToListAsync(ct);

        // Fetch distinct store information in parallel
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
                EstimatedDeliveryAt: o.EstimatedDeliveryAt,
                DeliveredAt: null,
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
