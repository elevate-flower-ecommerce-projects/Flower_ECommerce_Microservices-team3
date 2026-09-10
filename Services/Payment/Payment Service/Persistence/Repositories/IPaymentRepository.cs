using Blocks.Contracts.Interfaces;
using Payment_Service.Entities;

namespace Payment_Service.Persistence.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Payment?> GetByPaymobIntentionIdAsync(
        string intentionId,
        CancellationToken cancellationToken = default);

    Task<Payment?> GetByPaymobTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default);
}