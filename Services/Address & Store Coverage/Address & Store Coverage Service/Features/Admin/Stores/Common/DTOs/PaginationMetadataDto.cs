namespace Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs
{
    public sealed record PaginationMetadataDto(
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages,
        bool HasNextPage,
        bool HasPreviousPage);
}
