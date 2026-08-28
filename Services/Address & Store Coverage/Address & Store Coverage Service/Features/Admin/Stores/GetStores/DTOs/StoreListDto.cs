using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.DTOs
{
    public sealed record StoreListDto(
        IReadOnlyList<StoreDto> Items,
        PaginationMetadataDto Pagination);
}
