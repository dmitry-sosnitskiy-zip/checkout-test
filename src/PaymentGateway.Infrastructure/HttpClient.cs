using System.Text;
using System.Text.Json;

using PaymentGateway.Domain;

namespace PaymentGateway.Infrastructure;

public class HttpClient: IHttpClient, IDisposable
{
    private readonly System.Net.Http.HttpClient _httpClient = new();
    
    public async Task<TResponse> SendPostRequest<TResponse>(string address, object requestBody)
    {
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Base address should be an absolute URI", nameof(address));
        }
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(uri, jsonContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseContent) ?? throw new InvalidOperationException($"Failed to deserialize response into type {typeof(TResponse).Name}");
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}