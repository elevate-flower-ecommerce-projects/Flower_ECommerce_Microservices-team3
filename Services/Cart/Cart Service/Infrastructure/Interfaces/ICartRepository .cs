using Blocks.Contracts.Interfaces;
using Cart_Service.Entities;

namespace Cart_Service.Infrastructure.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}
