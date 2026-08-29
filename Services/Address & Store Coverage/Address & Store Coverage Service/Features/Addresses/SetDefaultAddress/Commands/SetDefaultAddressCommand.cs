using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.Commands
{
    public sealed record SetDefaultAddressCommand(
        Guid CustomerId,
        Guid AddressId
    ) : IRequest<Result<SetDefaultAddressResponseDto>>;
}
