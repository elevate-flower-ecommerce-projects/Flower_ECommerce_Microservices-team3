using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.DeleteAddress.Commands
{
    public sealed record DeleteAddressCommand(
       Guid AddressId,
       Guid CustomerId
   ) : IRequest<Result<string>>;
}
