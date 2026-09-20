using Blocks.Contracts.Common;
using MediatR;

namespace Order___Fulfillment_Service.Features.Checkout.EstimateDelivery;

public sealed record EstimateDeliveryQuery(
    Guid CustomerId,
    Guid AddressId,
    string? BearerToken = null
) : IRequest<Result<EstimateDeliveryResponse>>;
