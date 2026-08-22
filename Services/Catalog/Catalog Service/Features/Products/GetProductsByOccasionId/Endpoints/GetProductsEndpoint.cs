using Blocks.Contracts.Common;
using Blocks.Contracts.Http;
using Blocks.Contracts.Pagination;
using Blocks.Domain.Errors;
using Catalog_Service.Features.Products.GetProductsByOccasionId.Queries;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products.GetProductsByOccasionId.Endpoints
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
                [FromQuery] Guid? occasionId,
                [FromQuery] int? pageNumber,
                [FromQuery] int? pageSize,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (!occasionId.HasValue || occasionId.Value == Guid.Empty)
                {
                    return Results.Json(
                        ApiResponse<PagedResult<ProductViewModel>>.Fail(Error.Validation("Occasion Id is required to filter products.", "occasionId")),
                        statusCode: StatusCodes.Status400BadRequest);
                }

                var query = new GetProductsQuery(occasionId.Value, pageNumber ?? 1, pageSize ?? 10);

                var result = await sender.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<PagedResult<ProductViewModel>>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<PagedResult<ProductViewModel>>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            })
            .WithName("GetProductsByOccasion")
            .WithTags("Products")
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<PagedResult<ProductViewModel>>>(StatusCodes.Status422UnprocessableEntity)
            .WithSummary("Get a paginated list of products for a specific occasion")
            .WithDescription("Retrieves products filtered by the required occasionId. Returns an empty list if no products are found.");
        }
    }
}
