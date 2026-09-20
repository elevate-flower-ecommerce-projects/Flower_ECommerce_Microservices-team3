using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.DTOs;

public sealed record UpdateOrderStatusRequestDto(DriverStatusUpdate NewStatus);
