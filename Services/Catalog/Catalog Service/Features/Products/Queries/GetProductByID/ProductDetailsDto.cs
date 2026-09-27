using System.Text.Json.Serialization;

namespace Catalog_Service.Features.Products.Queries.GetProductByID;

public sealed class ProductDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Currency { get; init; } = "EGP";
    public decimal Price { get; init; }
    public decimal OriginalPrice { get; init; }
    public int DiscountPercentage { get; init; }
    public string Status { get; init; } = "InStock";
    public List<string> Images { get; init; } = [];
    public string Description { get; init; } = string.Empty;
    public List<ProductIncludeDto> Includes { get; init; } = [];
}

public sealed class ProductIncludeDto
{
    public string Name { get; init; } = string.Empty;
}

public sealed class ProductDetailsApiResponse
{
    public ProductDetailsDto? Data { get; init; }
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public string MessageLocalized { get; init; } = string.Empty;
    public string StatusCode { get; init; } = "Success";

    public static ProductDetailsApiResponse Success(
        ProductDetailsDto data,
        string message = "Product retrieved successfully",
        string? messageLocalized = null)
    {
        return new ProductDetailsApiResponse
        {
            Data = data,
            IsSuccess = true,
            Message = message,
            MessageLocalized = messageLocalized ?? message,
            StatusCode = "Success"
        };
    }

    public static ProductDetailsApiResponse Failure(
        string message,
        string statusCode = "NotFound",
        string? messageLocalized = null)
    {
        return new ProductDetailsApiResponse
        {
            Data = null,
            IsSuccess = false,
            Message = message,
            MessageLocalized = messageLocalized ?? message,
            StatusCode = statusCode
        };
    }
}
