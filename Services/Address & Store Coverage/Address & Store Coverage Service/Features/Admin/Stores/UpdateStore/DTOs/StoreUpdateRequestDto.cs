using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.DTOs
{
    public sealed record StoreUpdateRequestDto(
        string? Name,
        GeoLocationDto? Location,
        bool? IsActive);
}
