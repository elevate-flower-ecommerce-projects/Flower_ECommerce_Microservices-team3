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

    private static readonly System.Text.Json.JsonSerializerOptions IntentionJsonOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<PaymobIntentionResponse> CreateIntentionAsync(
        PaymobIntentionRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/intention/")
        {
            Content = JsonContent.Create(request, options: IntentionJsonOptions)
        };

        if (!string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Token", _options.SecretKey);
        }

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Paymob returned {(int)response.StatusCode} {response.ReasonPhrase}: {errorBody}");
        }

        var result =
            await response.Content.ReadFromJsonAsync<PaymobIntentionResponse>(
                cancellationToken: cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "Paymob returned an empty response.");
    }

    public async Task<PaymobInquiryResult?> InquireTransactionAsync(
        string paymobOrderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(paymobOrderId))
        {
            return null;
        }

        try
        {
            using var authReq = new HttpRequestMessage(HttpMethod.Post, "api/auth/tokens")
            {
                Content = JsonContent.Create(new { api_key = _options.ApiKey })
            };
            using var authRes = await _httpClient.SendAsync(authReq, cancellationToken);
            if (!authRes.IsSuccessStatusCode)
            {
                return null;
            }

            var tokenDoc = await authRes.Content.ReadFromJsonAsync<System.Text.Json.JsonDocument>(cancellationToken: cancellationToken);
            if (tokenDoc is null || !tokenDoc.RootElement.TryGetProperty("token", out var tokenEl))
            {
                return null;
            }

            var authToken = tokenEl.GetString();

            object inqPayload = int.TryParse(paymobOrderId, out var intOrderId)
                ? new { auth_token = authToken, order_id = intOrderId }
                : (object)new { auth_token = authToken, merchant_order_id = paymobOrderId };

            using var inqReq = new HttpRequestMessage(HttpMethod.Post, "api/ecommerce/orders/transaction_inquiry")
            {
                Content = JsonContent.Create(inqPayload)
            };
            using var inqRes = await _httpClient.SendAsync(inqReq, cancellationToken);
            if (!inqRes.IsSuccessStatusCode)
            {
                return null;
            }

            var inqDoc = await inqRes.Content.ReadFromJsonAsync<System.Text.Json.JsonDocument>(cancellationToken: cancellationToken);
            if (inqDoc is null) return null;

            var root = inqDoc.RootElement;
            bool success = false;
            string? txId = null;

            if (root.TryGetProperty("success", out var succEl) && succEl.ValueKind is System.Text.Json.JsonValueKind.True or System.Text.Json.JsonValueKind.False)
            {
                success = succEl.GetBoolean();
            }

            if (root.TryGetProperty("id", out var idEl))
            {
                txId = idEl.ToString();
            }

            return new PaymobInquiryResult(success, txId, paymobOrderId);
        }
        catch
        {
            return null;
        }
    }
}