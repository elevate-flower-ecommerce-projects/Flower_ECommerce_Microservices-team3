using Catalog_Service.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Categories.Queries.GetActiveCategories;

public class GetActiveCategoriesQueryHandler
    : IRequestHandler<GetActiveCategoriesQuery, List<CategoryDto>>
{
    private readonly FlowersCatalogDbContext _context;

    public GetActiveCategoriesQueryHandler(
        FlowersCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(
        GetActiveCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var isArabic = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = isArabic && !string.IsNullOrWhiteSpace(c.NameAr) ? c.NameAr : c.Name,
                Icon = c.Icon
            })
            .ToListAsync(cancellationToken);
    }
}