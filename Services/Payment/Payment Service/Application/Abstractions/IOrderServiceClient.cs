namespace Payment_Service.Application.Abstractions;

public interface IOrderServiceClient
{
    Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);
}
