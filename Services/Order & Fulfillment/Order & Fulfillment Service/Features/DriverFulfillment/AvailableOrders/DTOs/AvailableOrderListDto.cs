using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;

public sealed record AvailableOrderListDto(
    List<DriverOrderSummaryDto> Items,
    PaginationMetadataDto Pagination);
