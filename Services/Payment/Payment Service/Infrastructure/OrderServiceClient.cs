using Microsoft.Extensions.Logging;
using Payment_Service.Application.Abstractions;

namespace Payment_Service.Infrastructure;

public sealed class OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger) : IOrderServiceClient
{
    public async Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsync($"/internal/orders/{orderId}/mark-paid", null, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Order service returned {StatusCode} when marking order {OrderId} as paid.", (int)response.StatusCode, orderId);
                return false;
            }

            logger.LogInformation("Successfully notified Order service that order {OrderId} is paid.", orderId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to notify Order service that order {OrderId} is paid.", orderId);
            return false;
        }
    }
}
