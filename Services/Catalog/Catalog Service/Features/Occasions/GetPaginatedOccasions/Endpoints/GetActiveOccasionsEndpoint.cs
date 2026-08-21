using Blocks.Contracts.Http;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.Queries;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.ViewModels;
using MediatR;

namespace Catalog_Service.Features.Occasions.GetPaginatedOccasions.Endpoints
{
    public static class GetActiveOccasionsEndpoint
    {
        public static void MapGetActiveOccasionsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/occasions", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetActiveOccasionsQuery(pageNumber ?? 1, pageSize ?? 10);

                var result = await sender.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<PagedResult<OccasionViewModel>>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<PagedResult<OccasionViewModel>>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            })
            .WithName("GetActiveOccasions")
            .WithTags("Occasions")
            .Produces<ApiResponse<PagedResult<OccasionViewModel>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PagedResult<OccasionViewModel>>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<PagedResult<OccasionViewModel>>>(StatusCodes.Status422UnprocessableEntity)
            .WithSummary("Get all active occasions (Paginated)")
            .WithDescription("Retrieves a paginated list of all active occasions. Returns localized name, image url, and pagination metadata.");
        }
    }
}
