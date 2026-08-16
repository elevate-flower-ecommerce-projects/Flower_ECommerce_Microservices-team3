using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Pagination;
using Catalog_Service.Entities;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.Queries;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Catalog_Service.Features.Occasions.GetPaginatedOccasions.Queries.Handlers
{
    public class GetActiveOccasionsQueryHandler : IRequestHandler<GetActiveOccasionsQuery, Result<PagedResult<OccasionViewModel>>>
    {
        private readonly IGenericRepository<Occasion> _repository;

        public GetActiveOccasionsQueryHandler(IGenericRepository<Occasion> repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<OccasionViewModel>>> Handle(GetActiveOccasionsQuery request, CancellationToken cancellationToken)
        {
            var isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var baseQuery = _repository.GetQueryable().OrderBy(o => o.Name);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var occasions = await baseQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OccasionViewModel(
                    o.Id,
                    isArabic && !string.IsNullOrWhiteSpace(o.NameAr) ? o.NameAr : o.Name,
                    o.ImageUrl
                ))
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<OccasionViewModel>
            {
                Items = occasions,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return Result.Success(pagedResult);
        }
    }
}
