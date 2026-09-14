namespace Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

public sealed record CoordinatesDto(double Lat, double Lng);

public sealed record DriverOrderItemDto(Guid ProductId, string ProductName, int Quantity);

public sealed record DriverPickupPointDto(
    string StoreName,
    CoordinatesDto Location,
    string Address);

public sealed record DriverDropoffPointDto(
    string RecipientName,
    string RecipientPhone,
    CoordinatesDto Location,
    string Address);

public sealed record DriverOrderDetailDto(
    Guid OrderId,
    string Status,
    bool IsGift,
    List<DriverOrderItemDto> Items,
    DriverPickupPointDto Pickup,
    DriverDropoffPointDto UserAddress);
