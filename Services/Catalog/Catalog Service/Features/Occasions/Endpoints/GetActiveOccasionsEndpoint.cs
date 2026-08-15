using Catalog_Service.Features.Occasions.Queries;
using MediatR;

namespace Catalog_Service.Features.Occasions.Endpoints
{
    public static class GetActiveOccasionsEndpoint
    {
        public static void MapGetActiveOccasionsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/occasions", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetActiveOccasionsQuery();
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
            .WithSummary("Get all active occasions")
            .WithDescription("Retrieves a list of all active occasions. Returns localized name and image url.");
        }
    }
}
