using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.Commands
{
    public sealed record UpdateStoreCommand(
        Guid Id,
        string? Name,
        GeoLocationDto? Location,
        bool? IsActive) : IRequest<Result<StoreDto>>;
}
