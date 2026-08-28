using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById.Queries
{
    public sealed record GetStoreByIdQuery(Guid Id) : IRequest<Result<StoreDto>>;
}
