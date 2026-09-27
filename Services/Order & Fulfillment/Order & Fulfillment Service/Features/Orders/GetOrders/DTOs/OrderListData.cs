namespace Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs;

public sealed record OrderListData(
    IReadOnlyList<OrderListItemDto> Items,
    PaginationDto Pagination
);

public sealed record PaginationDto(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);
