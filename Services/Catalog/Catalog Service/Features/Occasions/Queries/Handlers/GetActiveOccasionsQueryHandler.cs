using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Catalog_Service.Entities;
using Catalog_Service.Features.Occasions.ViewModels;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Globalization;

namespace Catalog_Service.Features.Occasions.Queries.Handlers
{
    public class GetActiveOccasionsQueryHandler : IRequestHandler<GetActiveOccasionsQuery, Result<IReadOnlyList<OccasionViewModel>>>
    {
        private readonly IGenericRepository<Occasion> _repository;

        public GetActiveOccasionsQueryHandler(IGenericRepository<Occasion> repository)
        {
            _repository = repository;
        }

        public async Task<Result<IReadOnlyList<OccasionViewModel>>> Handle(GetActiveOccasionsQuery request, CancellationToken cancellationToken)
        {
            var isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            var occasions = await _repository.GetQueryable()
                .OrderBy(o => o.Name)
                .Select(o => new OccasionViewModel(
                    o.Id,
                    isArabic && !string.IsNullOrWhiteSpace(o.NameAr) ? o.NameAr : o.Name,
                    o.ImageUrl
                ))
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<OccasionViewModel>>(occasions);
        }
    }
}
