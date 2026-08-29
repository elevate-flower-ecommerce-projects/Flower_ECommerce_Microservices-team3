using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById.Queries
{
    public sealed class GetStoreByIdQueryHandler(
        IGenericRepository<Store> storeRepository)
        : IRequestHandler<GetStoreByIdQuery, Result<StoreDto>>
    {
        public async Task<Result<StoreDto>> Handle(
            GetStoreByIdQuery request,
            CancellationToken cancellationToken)
        {
            var store = await storeRepository.GetQueryable()
                .AsNoTracking()
                .Where(s => s.Id == request.Id && s.DeletedAt == null)
                .Select(s => new StoreDto(
                    s.Id,
                    s.Name,
                    new GeoLocationDto(s.Latitude, s.Longitude),
                    s.IsActive,
                    s.CreatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (store is null)
            {
                return Result.Failure<StoreDto>(Error.NotFound("Store not found."));
            }

            return Result.Success(store);
        }
    }
}
