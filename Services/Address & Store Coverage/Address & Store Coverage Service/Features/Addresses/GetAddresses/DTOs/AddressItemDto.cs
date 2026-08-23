namespace Address___Store_Coverage_Service.Features.Addresses.GetAddresses.DTOs
{
    public sealed record AddressItemDto(
        Guid Id,
        string RecipientName,
        string RecipientPhone,
        string AddressLine,
        Guid CityId,
        Guid AreaId,
        double Lat,
        double Lng,
        string? Label,
        bool IsDefault,
        Guid StoreId,
        DateTime CreatedAt
    );
}
