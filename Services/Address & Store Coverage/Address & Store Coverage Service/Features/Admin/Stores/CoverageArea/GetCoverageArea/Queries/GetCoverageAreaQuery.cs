using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.Common.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.GetCoverageArea.Queries
{
    public sealed record GetCoverageAreaQuery(Guid StoreId) : IRequest<Result<CoverageAreaDto>>;
}
