using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.Queries
{
    public sealed class GetStoresQueryHandler(
        IGenericRepository<Store> storeRepository)
        : IRequestHandler<GetStoresQuery, Result<StoreListDto>>
    {
        public async Task<Result<StoreListDto>> Handle(
            GetStoresQuery request,
            CancellationToken cancellationToken)
        {
            var query = storeRepository.GetQueryable()
                .AsNoTracking()
                .Where(s => s.DeletedAt == null);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(s => s.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new StoreDto(
                    s.Id,
                    s.Name,
                    new GeoLocationDto(s.Latitude, s.Longitude),
                    s.IsActive,
                    s.CreatedAt))
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
            var hasNextPage = request.Page < totalPages;
            var hasPreviousPage = request.Page > 1;

            var pagination = new PaginationMetadataDto(
                Page: request.Page,
                PageSize: request.PageSize,
                TotalCount: totalCount,
                TotalPages: totalPages,
                HasNextPage: hasNextPage,
                HasPreviousPage: hasPreviousPage);

            return Result.Success(new StoreListDto(items, pagination));
        }
    }
}
