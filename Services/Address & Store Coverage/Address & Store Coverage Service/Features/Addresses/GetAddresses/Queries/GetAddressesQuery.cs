using Address___Store_Coverage_Service.Features.Addresses.GetAddresses.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.GetAddresses.Queries
{
    public sealed record GetAddressesQuery(
         Guid CustomerId
     ) : IRequest<Result<List<AddressItemDto>>>;
}
