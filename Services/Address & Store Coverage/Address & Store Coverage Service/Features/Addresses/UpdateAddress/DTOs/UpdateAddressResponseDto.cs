namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.DTOs
{
    public sealed record UpdateAddressResponseDto(
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
