using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Pagination;
using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Catalog_Service.Features.Products.ViewModels;
using MediatR;
using System.Globalization;

namespace Catalog_Service.Features.Products.Queries.Handlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductViewModel>>>
    {
        private readonly IGenericRepository<Product> _repository;

        public GetProductsQueryHandler(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<ProductViewModel>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var baseQuery = _repository.GetQueryable();

            if (request.OccasionId.HasValue)
            {
                baseQuery = baseQuery.Where(p => p.ProductOccasions.Any(po => po.OccasionId == request.OccasionId.Value));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var products = await baseQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductViewModel(
                    p.Id,
                    isArabic && !string.IsNullOrWhiteSpace(p.NameAr) ? p.NameAr : p.Name,
                    p.ImageUrl,
                    p.Currency,
                    p.Price,
                    p.OriginalPrice,
                    p.DiscountPercentage,
                    p.Status.ToString(),
                    p.IsBestSeller
                ))
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<ProductViewModel>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return Result.Success(pagedResult);
        }
    }
}
