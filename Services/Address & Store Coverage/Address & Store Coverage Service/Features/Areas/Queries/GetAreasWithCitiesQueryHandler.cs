using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Areas.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Areas.Queries
{
    public sealed class GetAreasWithCitiesQueryHandler(
        IGenericRepository<Area> areaRepository)
        : IRequestHandler<GetAreasWithCitiesQuery, Result<List<AreaWithCitiesDto>>>
    {
        public async Task<Result<List<AreaWithCitiesDto>>> Handle(
            GetAreasWithCitiesQuery request,
            CancellationToken cancellationToken)
        {
            var areas = await areaRepository.GetQueryable()
                .AsNoTracking()
                .Where(a => a.DeletedAt == null && a.IsActive)
                .OrderBy(a => a.Name)
                .Select(a => new AreaWithCitiesDto(
                    a.Id,
                    a.Name,
                    a.Cities
                        .Where(c => c.DeletedAt == null && c.IsActive)
                        .OrderBy(c => c.Name)
                        .Select(c => new CityDto(c.Id, c.Name))
                        .ToList()))
                .ToListAsync(cancellationToken);

            return Result.Success(areas);
        }
    }
}
