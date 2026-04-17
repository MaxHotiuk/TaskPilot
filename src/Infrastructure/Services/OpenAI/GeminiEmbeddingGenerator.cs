using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using System.Net.Http.Json;

namespace Infrastructure.Services.OpenAI;

/// <summary>
/// KernelMemory ITextEmbeddingGenerator implementation backed by the
/// Gemini text-embedding-004 REST API.  Used only for vector indexing and
/// search — the chat completion path uses ChatService with its own HttpClient.
/// </summary>
internal sealed class GeminiEmbeddingGenerator : ITextEmbeddingGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    // gemini-embedding-001: 3072-dimensional output, 2 048-token input window
    private const string Model = "gemini-embedding-001";
    private const string EndpointTemplate =
        "https://generativelanguage.googleapis.com/v1beta/models/{0}:embedContent?key={1}";

    public int MaxTokens => 2048;

    // ITextTokenizer — approximate token count using whitespace split
    public int CountTokens(string text) =>
        string.IsNullOrWhiteSpace(text) ? 0 : text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    public IReadOnlyList<string> GetTokens(string text) =>
        string.IsNullOrWhiteSpace(text)
            ? []
            : text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    public GeminiEmbeddingGenerator(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<Embedding> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var url = string.Format(EndpointTemplate, Model, _apiKey);

        var requestBody = new GeminiEmbeddingRequest(
            Model: $"models/{Model}",
            Content: new GeminiContent(Parts: [new GeminiPart(Text: text)])
        );

        var httpResponse = await _httpClient.PostAsJsonAsync(url, requestBody, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var result = await httpResponse.Content.ReadFromJsonAsync<GeminiEmbeddingResponse>(
                         cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException(
                            "Received a null response from the Gemini Embedding API.");

        // Embedding has an implicit cast operator from float[]
        return result.Embedding.Values;
    }
}
