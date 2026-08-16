using Blocks.Contracts.Http;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog_Service.Features.Products.GetProductById;

public static class GetProductByIdEndpoint
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{productId:guid}", async (
            Guid productId,
            [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var language = (acceptLanguage?.StartsWith("ar", StringComparison.OrdinalIgnoreCase) == true)
                ? "ar"
                : "en";

            var result = await mediator.Send(new GetProductByIdQuery(productId, language), ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<ProductDetailsDto>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<ProductDetailsDto>.Ok(result.Value));
        })
        .WithName("GetProductById")
        .WithTags("Products")
        .Produces<ApiResponse<ProductDetailsDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ProductDetailsDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ProductDetailsDto>>(StatusCodes.Status422UnprocessableEntity);

        return app;
    }
}
