using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Cart_Service.Entities;
using Cart_Service.Features.GetCart.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.GetCart.Queries.Handlers;

public sealed class GetCartQueryHandler(
    IGenericRepository<Entities.Cart> cartRepository)
    : IRequestHandler<GetCartQuery, Result<GetCartResponse>>
{
    public async Task<Result<GetCartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var response = await cartRepository.GetQueryable()
            .AsNoTracking()
            .Where(c => c.CustomerId == request.CustomerId)
            .Select(c => new GetCartResponse(
                c.Id,
                c.CustomerId,
                c.Items.Select(i => new GetCartItemResponse(
                    i.Id,
                    i.ProductId,
                    "Fresh Flower Arrangement",
                    "categories/tulip_flower.png",
                    i.UnitPrice,
                    i.Quantity,
                    i.UnitPrice * i.Quantity,
                    true,
                    50,
                    false
                )).ToList(),
                c.Subtotal,
                null,
                c.Total,
                false
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.Success(new GetCartResponse(
                Guid.Empty,
                request.CustomerId,
                Array.Empty<GetCartItemResponse>(),
                0m,
                null,
                0m,
                false));
        }

        return Result.Success(response);
    }
}
