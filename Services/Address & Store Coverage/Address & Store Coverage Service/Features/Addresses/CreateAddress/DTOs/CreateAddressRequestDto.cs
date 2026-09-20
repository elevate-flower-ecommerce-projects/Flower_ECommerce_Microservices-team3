namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.DTOs
{
    public sealed record CreateAddressRequestDto(
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
