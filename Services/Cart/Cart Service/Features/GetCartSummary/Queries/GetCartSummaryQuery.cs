using Blocks.Contracts.Common;
using Cart_Service.Features.Cart.DTOs;
using MediatR;

namespace Cart_Service.Features.GetCartSummary.Queries;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder query for GetCartSummary (SCRUM-29 / SCRUM-107) until intern finishes.
// =========================================================================================================

public record GetCartSummaryQuery(
    Guid CustomerId,
    string Language
) : IRequest<Result<CartSummaryDto>>;
