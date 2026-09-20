using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore.DTOs
{
    public sealed record StoreCreateRequestDto(
        string Name,
        GeoLocationDto Location,
        bool IsActive = true);
}
