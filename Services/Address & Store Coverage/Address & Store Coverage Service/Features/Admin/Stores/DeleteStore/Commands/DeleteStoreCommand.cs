using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore.Commands
{
    public sealed record DeleteStoreCommand(Guid Id) : IRequest<Result<string>>;
}
