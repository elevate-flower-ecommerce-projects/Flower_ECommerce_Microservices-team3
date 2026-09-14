namespace Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

public sealed record StoreSummaryDto(string Name, string Address);

public sealed record RecipientSummaryDto(string Name, string City, string Area);

public sealed record DriverOrderSummaryDto(
    Guid OrderId,
    string Status,
    StoreSummaryDto Store,
    RecipientSummaryDto Recipient,
    int ItemCount,
    decimal Total,
    DateTime? EstimatedDeliveryAt = null,
    DateTime? DeliveredAt = null,
    DateTime? AssignedAt = null);
