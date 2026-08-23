using Address___Store_Coverage_Service.Entities;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Queries
{
    public sealed class ValidateCityAreaQueryHandler(
       IGenericRepository<Area> areaRepository)
       : IRequestHandler<ValidateCityAreaQuery, Result>
    {
        public async Task<Result> Handle(
            ValidateCityAreaQuery request,
            CancellationToken cancellationToken)
        {
            var area = await areaRepository.GetQueryable()
                .AsNoTracking()
                .Where(a => a.Id == request.AreaId && a.IsActive && a.DeletedAt == null)
                .Select(a => new
                {
                    a.CityId,
                    CityIsUsable = a.City.IsActive && a.City.DeletedAt == null
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (area is null)
                return Result.Failure(Error.NotFound("Area not found."));
            if (area.CityId != request.CityId)
                return Result.Failure(
                    Error.Validation("The selected area does not belong to the selected city.", "AreaId"));
            if (!area.CityIsUsable)
                return Result.Failure(Error.NotFound("City not found."));
            return Result.Success();
        }
    }
}
