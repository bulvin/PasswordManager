using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace PasswordManager.AI
{
    public interface IGroqApiClient
    {
        Task<JsonObject?> CreateChatCompletionAsync(JsonObject request);
        IAsyncEnumerable<JsonObject?> CreateChatCompletionStreamAsync(JsonObject request);
    }

    public class GroqApiClientOptions
    {
        public string ApiKey { get; init; } = default!;
        public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1";
    }

    public class GroqApiClient : IGroqApiClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private const string ChatCompletionsEndpoint = "/chat/completions";

        public GroqApiClient(HttpClient httpClient, IOptions<GroqApiClientOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (options?.Value == null)
                throw new ArgumentNullException(nameof(options));

            _baseUrl = options.Value.BaseUrl.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(options));

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
        }

        public async Task<JsonObject?> CreateChatCompletionAsync(JsonObject request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}{ChatCompletionsEndpoint}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<JsonObject>();
        }

        public async IAsyncEnumerable<JsonObject?> CreateChatCompletionStreamAsync(JsonObject request)
        {
            request["stream"] = true;
            var content = new StringContent(request.ToJsonString(), Encoding.UTF8, "application/json");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{ChatCompletionsEndpoint}") { Content = content };

            using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("data: "))
                {
                    var data = line["data: ".Length..];
                    if (data != "[DONE]")
                    {
                        yield return JsonSerializer.Deserialize<JsonObject>(data);
                    }
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
