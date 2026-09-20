using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Queries
{
    public sealed record ValidateCityAreaQuery(
        Guid CityId,
        Guid AreaId
    ) : IRequest<Result>;
}
