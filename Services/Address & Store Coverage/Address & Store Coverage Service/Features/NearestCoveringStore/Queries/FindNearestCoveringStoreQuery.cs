using Address___Store_Coverage_Service.Features.NearestCoveringStore.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.NearestCoveringStore.Queries
{
    public sealed record FindNearestCoveringStoreQuery(
        double Latitude,
        double Longitude
    ) : IRequest<Result<NearestStoreDto>>;
}
