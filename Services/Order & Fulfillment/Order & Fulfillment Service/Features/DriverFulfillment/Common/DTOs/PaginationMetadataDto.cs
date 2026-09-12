namespace Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

public sealed record PaginationMetadataDto(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
