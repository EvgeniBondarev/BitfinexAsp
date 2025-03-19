using System.Text;
using System.Text.Json;

namespace BitfinexAsp.ApiClients;

/// <summary>
/// Основной HTTP-клиент для отправки запросов к API.
/// </summary>
public class MainClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public MainClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    }

    public async Task<string> SendRequestAsync(HttpMethod method, string endpoint, object? content = null, string clientName = "default")
    {
        var client = _httpClientFactory.CreateClient(clientName);
        using var request = new HttpRequestMessage(method, endpoint);

        if (content != null)
        {
            string json = JsonSerializer.Serialize(content, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}
