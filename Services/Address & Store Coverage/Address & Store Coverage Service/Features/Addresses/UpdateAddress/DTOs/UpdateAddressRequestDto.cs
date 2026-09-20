namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.DTOs
{
    public sealed record UpdateAddressRequestDto(
       string RecipientName,
       string Phone,
       string AddressLine,
       Guid CityId,
       Guid AreaId,
       double Latitude,
       double Longitude,
       string? Label
   );
}
