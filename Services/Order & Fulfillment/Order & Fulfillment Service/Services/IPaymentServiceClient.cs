using Blocks.Contracts.Payment;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Services;

public record BillingData(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string Building,
    string Floor,
    string Apartment
);

public record CreatePaymentSessionRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    PaymentProvider PaymentProvider,
    DateTime EstimatedDeliveryAt,
    BillingData BillingData
);

public record CardSessionResultDto(
    Guid OrderId,
    OrderStatus Status,
    PaymentProvider PaymentProvider,
    string SessionId,
    string SessionUrl,
    string? SuccessUrl = null,
    string? CancelUrl = null,
    DateTime? ExpiresAt = null,
    decimal Amount = 0,
    string Currency = "EGP",
    DateTime? EstimatedDeliveryAt = null
);

public interface IPaymentServiceClient
{
    Task<CardSessionResultDto?> CreateCardSessionAsync(
        CreatePaymentSessionRequest request,
        string? bearerToken = null,
        CancellationToken ct = default);
}