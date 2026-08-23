using MediatR;

namespace Catalog_Service.Features.Categories.Queries.GetActiveCategories;

public record GetActiveCategoriesQuery
    : IRequest<List<CategoryDto>>;