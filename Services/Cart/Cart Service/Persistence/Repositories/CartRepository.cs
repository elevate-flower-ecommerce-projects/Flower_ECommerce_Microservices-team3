using Cart_Service.Entities;
using Cart_Service.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Persistence.Repositories
{
    public sealed class CartRepository
        : GenericRepository<Cart>, ICartRepository
    {
        private readonly FlowersCartDbContext _context;

        public CartRepository(FlowersCartDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Carts
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x => x.CustomerId == customerId,
                    cancellationToken);
        }
    }
}
