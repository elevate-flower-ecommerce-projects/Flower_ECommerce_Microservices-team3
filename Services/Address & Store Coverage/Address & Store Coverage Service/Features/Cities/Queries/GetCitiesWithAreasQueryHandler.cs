using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Cities.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Cities.Queries
{
    public sealed class GetCitiesWithAreasQueryHandler(
    IGenericRepository<City> cityRepository)
    : IRequestHandler<GetCitiesWithAreasQuery, Result<List<CityWithAreasDto>>>
    {
        public async Task<Result<List<CityWithAreasDto>>> Handle(
         GetCitiesWithAreasQuery request,
         CancellationToken cancellationToken)
        {
            var cities = await cityRepository.GetQueryable()
                .AsNoTracking()
                .Where(c => c.DeletedAt == null && c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CityWithAreasDto(
                    c.Id,
                    c.Name,
                    c.Areas
                        .Where(a => a.DeletedAt == null && a.IsActive)
                        .OrderBy(a => a.Name)
                        .Select(a => new AreaDto(a.Id, a.Name))
                        .ToList()))
                .ToListAsync(cancellationToken);

            return Result.Success(cities);
        }
    }
}
