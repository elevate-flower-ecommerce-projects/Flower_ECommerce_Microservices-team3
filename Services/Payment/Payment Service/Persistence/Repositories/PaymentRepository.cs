using Microsoft.EntityFrameworkCore;
using Payment_Service.Entities;
using Payment_Service.Persistence;
using Payment_Service.Persistence.Repositories;

namespace Payment_Service.Repositories;

public sealed class PaymentRepository(FlowersPaymentDbContext _context)
    : GenericRepository<Payment>(_context), IPaymentRepository
{
    public async Task<Payment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId,
                cancellationToken);
    }

    public async Task<Payment?> GetByPaymobIntentionIdAsync(
        string intentionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(
                x => x.PaymobIntentionId == intentionId,
                cancellationToken);
    }

    public async Task<Payment?> GetByPaymobTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(
                x => x.PaymobTransactionId == transactionId,
                cancellationToken);
    }
}