using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress
{
    public sealed record UpdateAddressOrchestrator(
       Guid AddressId,
       Guid CustomerId,
       string RecipientName,
       string Phone,
       string AddressLine,
       Guid CityId,
       Guid AreaId,
       double Latitude,
       double Longitude,
       string? Label
   ) : IRequest<Result<UpdateAddressResponseDto>>;
}
