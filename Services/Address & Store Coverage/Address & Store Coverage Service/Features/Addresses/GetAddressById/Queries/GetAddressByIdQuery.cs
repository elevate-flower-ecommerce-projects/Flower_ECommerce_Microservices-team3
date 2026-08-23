using Address___Store_Coverage_Service.Features.Addresses.GetAddressById.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.GetAddressById.Queries
{
    public sealed record GetAddressByIdQuery(
       Guid CustomerId,
       Guid AddressId
   ) : IRequest<Result<AddressDetailsDto>>;
}
