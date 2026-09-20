namespace Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs
{
    public sealed record StoreDto(
        Guid Id,
        string Name,
        GeoLocationDto Location,
        bool IsActive,
        DateTime CreatedAt);
}
