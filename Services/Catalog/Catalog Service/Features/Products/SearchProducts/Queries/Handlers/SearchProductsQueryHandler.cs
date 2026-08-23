using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Pagination;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Globalization;

namespace Catalog_Service.Features.Products.SearchProducts.Queries.Handlers
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<PagedResult<ProductViewModel>>>
    {
        private readonly IGenericRepository<Product> _repository;

        public SearchProductsQueryHandler(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<ProductViewModel>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var isArabic = request.Language == "ar";
            var keyword = request.Keyword.Trim();

            
            var query = _repository.GetQueryable()
                .Where(p => p.Name.Contains(keyword) || (p.NameAr != null && p.NameAr.Contains(keyword)));

            
            // if (request.StoreId.HasValue && request.StoreId.Value != Guid.Empty)
            // {
            //     query = query.Where(p => /* logic for store inventory */);
            // }

            var totalCount = await query.CountAsync(cancellationToken);

            
            var products = await query
                .OrderByDescending(p => p.Name.StartsWith(keyword) || (p.NameAr != null && p.NameAr.StartsWith(keyword)))
                .ThenBy(p => p.Name)
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
