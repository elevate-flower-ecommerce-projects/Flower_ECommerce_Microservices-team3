using Blocks.Contracts.Common;
using MediatR;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public sealed record GetCheckoutDetailsQuery(
    Guid CustomerId,
    string? BearerToken = null
) : IRequest<Result<CheckoutDetailsResponse>>;
