using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Services;

public record CreatePaymentSessionRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    PaymentGateway Gateway,
    DateTime EstimatedDeliveryAt
);

public record CardSessionResultDto(
    Guid OrderId,
    OrderStatus Status,
    PaymentGateway Gateway,
    string SessionId,
    string SessionUrl,
    string SuccessUrl,
    string CancelUrl,
    DateTime ExpiresAt,
    decimal Amount,
    string Currency,
    DateTime EstimatedDeliveryAt
);

public interface IPaymentServiceClient
{
    Task<CardSessionResultDto?> CreateCardSessionAsync(
        CreatePaymentSessionRequest request,
        string? bearerToken = null,
        CancellationToken ct = default);
}
