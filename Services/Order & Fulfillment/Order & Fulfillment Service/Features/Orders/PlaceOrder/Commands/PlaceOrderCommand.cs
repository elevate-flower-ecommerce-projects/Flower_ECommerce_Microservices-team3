using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.PlaceOrder.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.PlaceOrder.Commands;

public sealed record PlaceOrderCommand(
    Guid CustomerId,
    string CustomerEmail,
    string CustomerName,
    string CustomerPhone,
    string BearerToken,
    PlaceOrderRequest Request
) : IRequest<Result<PlaceOrderCardResult?>>;
