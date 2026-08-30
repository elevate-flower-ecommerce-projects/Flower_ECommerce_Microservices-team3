using Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.Queries
{
    public sealed record GetStoresQuery(int Page = 1, int PageSize = 10) : IRequest<Result<StoreListDto>>;
}
