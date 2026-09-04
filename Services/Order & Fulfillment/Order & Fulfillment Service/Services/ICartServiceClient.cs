namespace Order___Fulfillment_Service.Services;

public record CartItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record CartDto(Guid Id, Guid CustomerId, decimal Subtotal, IReadOnlyList<CartItemDto> Items);

public interface ICartServiceClient
{
    Task<CartDto?> GetCartByIdAsync(Guid cartId, string? bearerToken = null, CancellationToken ct = default);
    Task<bool> ClearCartAsync(Guid cartId, string? bearerToken = null, CancellationToken ct = default);
}
