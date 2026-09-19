namespace Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs
{
    public sealed record OrderAddressDto(
    string RecipientName,
    string RecipientPhone,
    string AddressLine,
    string City,
    string Area);

}
