using System.Globalization;
using Blocks.Contracts.Http;
using Catalog_Service.Features.Products.GetProductByCategory.Querry;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products.GetProductByCategory.Endpoints;

public static class GetProductsByCategoryEndpoint
{
    public static IEndpointRouteBuilder MapGetProductsByCategoryEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/products/by-category",
            async (
                [FromQuery] Guid categoryId,
                [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var language = !string.IsNullOrWhiteSpace(acceptLanguage)
                    ? (acceptLanguage.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en")
                    : (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en");

                var result = await mediator.Send(
                    new GetProductsByCategoryQuery(categoryId, language),
                    cancellationToken);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<List<ProductDto>>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<List<ProductDto>>.Ok(result.Value));
            })
        .WithName("GetProductsByCategory")
        .WithTags("Products")
        .Produces<ApiResponse<List<ProductDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<List<ProductDto>>>(StatusCodes.Status404NotFound);

        return endpoints;
    }
}