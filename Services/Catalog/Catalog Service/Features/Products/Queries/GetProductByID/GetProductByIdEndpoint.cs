using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductByID
{
    public static class GetProductByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetProductByIdEndpoint(
            this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/products/{id:guid}",
                async (Guid id,
                       ISender sender,
                       CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new GetProductByIdQuery(id),
                        cancellationToken);

                    return result;
                });

            return app;
        }
    }
}
