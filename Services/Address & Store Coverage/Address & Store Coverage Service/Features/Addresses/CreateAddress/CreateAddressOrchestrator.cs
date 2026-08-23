using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress
{
    public sealed record CreateAddressOrchestrator(
        Guid CustomerId,
        string RecipientName,
        string Phone,
        string AddressLine,
        Guid CityId,
        Guid AreaId,
        double Latitude,
        double Longitude,
        string? Label
    ) : IRequest<Result<CreateAddressResponseDto>>;
}
