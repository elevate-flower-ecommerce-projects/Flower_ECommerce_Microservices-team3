using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Payment_Service.Application.Abstractions;

namespace Payment_Service.Infrastructure.Paymob;

public sealed class PaymobClient(HttpClient httpClient, IOptions<PaymobOptions> options)
    : IPaymobClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly PaymobOptions _options = options.Value;

    public async Task<PaymobIntentionResponse> CreateIntentionAsync(
        PaymobIntentionRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/intention/")
        {
            Content = JsonContent.Create(request)
        };

        if (!string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Token", _options.SecretKey);
        }

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<PaymobIntentionResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "Paymob returned an empty response.");
    }
}