using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore.Commands
{
    public sealed record CreateStoreCommand(
        string Name,
        double Latitude,
        double Longitude,
        bool IsActive = true) : IRequest<Result<StoreDto>>;
}
