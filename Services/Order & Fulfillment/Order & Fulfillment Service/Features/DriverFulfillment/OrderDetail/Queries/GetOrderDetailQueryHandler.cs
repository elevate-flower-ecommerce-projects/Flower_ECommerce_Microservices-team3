using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.OrderDetail.Queries;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.OrderDetail.Queries;

public sealed class GetOrderDetailQueryHandler(
    IGenericRepository<Order> orderRepository,
    IAddressServiceClient addressServiceClient)
    : IRequestHandler<GetOrderDetailQuery, Result<DriverOrderDetailDto>>
{
    public async Task<Result<DriverOrderDetailDto>> Handle(
        GetOrderDetailQuery request,
        CancellationToken ct)
    {
        var rawOrder = await orderRepository.GetQueryable()
            .AsNoTracking()
            .Where(o => o.Id == request.OrderId && o.AssignedDriverId == request.DriverId)
            .Select(o => new
            {
                o.Id,
                o.Status,
                o.IsGift,
                o.StoreId,
                o.RecipientName,
                o.RecipientPhone,
                o.GiftRecipientName,
                o.GiftRecipientPhone,
                o.DeliveryLatitude,
                o.DeliveryLongitude,
                o.AddressLine,
                Items = o.Items.Select(i => new DriverOrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.Quantity)).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (rawOrder is null)
        {
            return Result.Failure<DriverOrderDetailDto>(DriverErrors.OrderNotFound());
        }

        var store = await addressServiceClient.GetStoreByIdAsync(rawOrder.StoreId, ct);

        var recipientName = rawOrder.IsGift && !string.IsNullOrWhiteSpace(rawOrder.GiftRecipientName)
            ? rawOrder.GiftRecipientName
            : rawOrder.RecipientName;

        var recipientPhone = rawOrder.IsGift && !string.IsNullOrWhiteSpace(rawOrder.GiftRecipientPhone)
            ? rawOrder.GiftRecipientPhone
            : rawOrder.RecipientPhone;

        var detail = new DriverOrderDetailDto(
            rawOrder.Id,
            rawOrder.Status.ToString(),
            rawOrder.IsGift,
            rawOrder.Items,
            new DriverPickupPointDto(
                store?.Name ?? "Unknown Store",
                new CoordinatesDto(store?.Lat ?? 0, store?.Lng ?? 0),
                store is not null ? $"{store.Lat}, {store.Lng}" : "Store Location"),
            new DriverDropoffPointDto(
                recipientName,
                recipientPhone,
                new CoordinatesDto(rawOrder.DeliveryLatitude, rawOrder.DeliveryLongitude),
                rawOrder.AddressLine));

        return Result.Success(detail);
    }
}
