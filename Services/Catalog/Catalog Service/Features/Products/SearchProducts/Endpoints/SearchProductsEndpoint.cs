using Blocks.Contracts.Http;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using Catalog_Service.Features.Products.SearchProducts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Catalog_Service.Features.Products.SearchProducts.Endpoints
{
    public static class SearchProductsEndpoint
    {
        public static void MapSearchProductsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products/search", async (
                [FromQuery(Name = "Keyword")] string keyword,
                [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
                [FromQuery] Guid? storeId,
                [FromQuery] int? pageNumber,
                [FromQuery] int? pageSize,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                var language = !string.IsNullOrWhiteSpace(acceptLanguage)
                ? (acceptLanguage.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en")
                : (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en");

                var query = new SearchProductsQuery(keyword, language, storeId, pageNumber ?? 1, pageSize ?? 10);

                var result = await sender.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<PagedResult<ProductViewModel>>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<PagedResult<ProductViewModel>>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            })
            .WithName("SearchProducts")
            .WithTags("Products")
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status422UnprocessableEntity)
            .WithSummary("Search for products by name")
            .WithDescription("Searches for products using a keyword. Supports Arabic and English with relevance sorting.");
        }
    }
}
