using System.Globalization;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Home.GetSections;

internal sealed class GetHomeSectionsHandler(
    IGenericRepository<HomeSection> homeSectionRepo)
    : IRequestHandler<GetHomeSectionsQuery, Result<List<HomeSectionResponse>>>
{
    public async Task<Result<List<HomeSectionResponse>>> Handle(
        GetHomeSectionsQuery request,
        CancellationToken cancellationToken)
    {
        var isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);

        var sections = await homeSectionRepo.GetQueryable()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Index)
            .AsNoTracking()
            .Select(s => new HomeSectionResponse(
                s.Id,
                s.Type.ToString(),
                s.Index,
                s.IsActive,
                isArabic && s.TitleAr != null && s.TitleAr != "" ? s.TitleAr : s.Title,
                s.OccasionId,
                s.CategoryId))
            .ToListAsync(cancellationToken);

        return Result.Success(sections);
    }
}
