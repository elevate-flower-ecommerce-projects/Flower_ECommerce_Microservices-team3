using System.Globalization;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductByID
{
    public static class GetProductByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetProductByIdEndpoint(
            this IEndpointRouteBuilder app)
        {
            var handler = async (
                string id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");

                if (!Guid.TryParse(id, out var productId))
                {
                    var notFoundMsg = "Product not found";
                    var notFoundMsgAr = isArabic ? "المنتج غير موجود" : notFoundMsg;

                    return Results.Json(
                        ProductDetailsApiResponse.Failure(notFoundMsg, "NotFound", notFoundMsgAr),
                        statusCode: StatusCodes.Status404NotFound);
                }

                var result = await sender.Send(
                    new GetProductByIdQuery(productId),
                    cancellationToken);

                if (result.IsFailure)
                {
                    var notFoundMsg = result.Error?.Message ?? "Product not found";
                    var notFoundMsgAr = isArabic ? "المنتج غير موجود" : notFoundMsg;

                    return Results.Json(
                        ProductDetailsApiResponse.Failure(notFoundMsg, "NotFound", notFoundMsgAr),
                        statusCode: StatusCodes.Status404NotFound);
                }

                var successMsg = "Product retrieved successfully";
                var successMsgAr = isArabic ? "تم استرجاع تفاصيل المنتج بنجاح" : successMsg;

                return Results.Ok(
                    ProductDetailsApiResponse.Success(result.Value, successMsg, successMsgAr));
            };

            app.MapGet("/products/{id}", handler)
                .WithName("GetProductById")
                .WithTags("Products")
                .WithSummary("Get Product by ID")
                .WithDescription("Retrieves product details including images, includes, description, stock status, and pricing.")
                .Produces<ProductDetailsApiResponse>(StatusCodes.Status200OK)
                .Produces<ProductDetailsApiResponse>(StatusCodes.Status404NotFound);

            app.MapGet("/api/v1/products/{id}", handler).ExcludeFromDescription();
            app.MapGet("/api/products/{id}", handler).ExcludeFromDescription();

            return app;
        }
    }
}
