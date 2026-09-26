using Blocks.Contracts.Payment;

namespace Payment_Service.Features.Commands.CreateCheckoutSession;

public sealed record CreateCheckoutSessionRequest(
    Guid OrderId,
    decimal Amount,
    string? Currency = "EGP",
    DateTime? EstimatedDeliveryAt = null,
    BillingData? BillingData = null,
    string? CustomerEmail = null,
    string? CustomerFirstName = null,
    string? CustomerLastName = null,
    string? CustomerPhone = null,
    string? Country = null,
    string? City = null,
    string? Street = null,
    string? Building = null,
    string? Floor = null,
    string? Apartment = null
);
