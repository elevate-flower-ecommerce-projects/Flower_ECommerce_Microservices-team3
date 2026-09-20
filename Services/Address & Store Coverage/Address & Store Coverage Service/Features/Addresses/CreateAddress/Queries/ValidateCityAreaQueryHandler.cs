using Address___Store_Coverage_Service.Entities;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Queries
{
    public sealed class ValidateCityAreaQueryHandler(
       IGenericRepository<City> cityRepository)
       : IRequestHandler<ValidateCityAreaQuery, Result>
    {
        public async Task<Result> Handle(
            ValidateCityAreaQuery request,
            CancellationToken cancellationToken)
        {
            var city = await cityRepository.GetQueryable()
                .AsNoTracking()
                .Where(c => c.Id == request.CityId && c.IsActive && c.DeletedAt == null)
                .Select(c => new
                {
                    c.AreaId,
                    AreaIsUsable = c.Area.IsActive && c.Area.DeletedAt == null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (city is null)
                return Result.Failure(Error.NotFound("City not found."));

            if (city.AreaId != request.AreaId)
                return Result.Failure(
                    Error.Validation("The selected city does not belong to the selected area.", "CityId"));

            if (!city.AreaIsUsable)
                return Result.Failure(Error.NotFound("Area not found."));

            return Result.Success();
        }
    }
}
