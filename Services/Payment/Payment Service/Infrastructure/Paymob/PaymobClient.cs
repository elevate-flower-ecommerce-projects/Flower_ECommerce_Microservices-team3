using System.Net.Http.Json;
using Payment_Service.Application.Abstractions;

namespace Payment_Service.Infrastructure.Paymob;

public sealed class PaymobClient(HttpClient httpClient)
    : IPaymobClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<PaymobIntentionResponse> CreateIntentionAsync(
        PaymobIntentionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "v1/intention/",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<PaymobIntentionResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "Paymob returned an empty response.");
    }
}