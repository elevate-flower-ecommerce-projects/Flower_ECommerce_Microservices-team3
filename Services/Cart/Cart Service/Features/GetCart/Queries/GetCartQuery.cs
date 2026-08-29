using Blocks.Contracts.Common;
using Cart_Service.Features.GetCart.ViewModels;
using MediatR;

namespace Cart_Service.Features.GetCart.Queries
{
    public record GetCartQuery(Guid CustomerId) : IRequest<Result<GetCartResponse>>;
}
