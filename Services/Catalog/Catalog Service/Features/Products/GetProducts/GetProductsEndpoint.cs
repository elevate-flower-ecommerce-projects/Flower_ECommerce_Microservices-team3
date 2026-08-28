using Blocks.Contracts.Http;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.GetProducts;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Catalog_Service.Features.Products.GetProducts
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
                [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
                [FromQuery] Guid? occasionId,
                [FromQuery] Guid? categoryId,
                [FromQuery] string? keyword,
                [FromQuery] string? sortBy,
                [FromQuery] int? page,
                [FromQuery] int? pageNumber,
                [FromQuery] int? pageSize,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                var language = !string.IsNullOrWhiteSpace(acceptLanguage)
                    ? (acceptLanguage.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en")
                    : (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en");

                var actualPage = page ?? pageNumber ?? 1;
                var actualPageSize = pageSize ?? 10;

                var query = new GetProductsQuery(
                    OccasionId: occasionId,
                    CategoryId: categoryId,
                    Keyword: keyword,
                    SortBy: sortBy,
                    Language: language,
                    Page: actualPage,
                    PageSize: actualPageSize
                );

                var result = await sender.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<PagedResult<ProductViewModel>>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<PagedResult<ProductViewModel>>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            })
            .WithName("GetProducts")
            .WithTags("Products")
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status422UnprocessableEntity)
            .WithSummary("Get products (paginated, filterable, searchable)")
            .WithDescription("Returns a paginated list of products filterable by category, occasion, keyword, and sort order.");
        }
    }
}
