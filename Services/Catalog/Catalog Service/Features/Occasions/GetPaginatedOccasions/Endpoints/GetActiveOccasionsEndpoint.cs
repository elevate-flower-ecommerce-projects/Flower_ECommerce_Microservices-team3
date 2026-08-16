using Catalog_Service.Features.Occasions.GetPaginatedOccasions.Queries;
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
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .WithName("GetActiveOccasions")
            .WithTags("Occasions")
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Get all active occasions (Paginated)")
            .WithDescription("Retrieves a paginated list of all active occasions. Returns localized name, image url, and pagination metadata.");
        }
    }
}
