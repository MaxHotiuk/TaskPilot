using System.Text.Json.Serialization;

namespace Infrastructure.Services.OpenAI;

// ── Text generation (generateContent) ────────────────────────────────────────

internal record GeminiRequest(
    [property: JsonPropertyName("contents")] List<GeminiContent> Contents);

internal record GeminiContent(
    [property: JsonPropertyName("parts")] List<GeminiPart> Parts);

internal record GeminiPart(
    [property: JsonPropertyName("text")] string Text);

internal record GeminiResponse(
    [property: JsonPropertyName("candidates")] List<GeminiCandidate>? Candidates);

internal record GeminiCandidate(
    [property: JsonPropertyName("content")] GeminiContent? Content);

// ── Embeddings (embedContent) ─────────────────────────────────────────────────

// NOTE: the embedding API uses a single `content` object (not a `contents` list)
internal record GeminiEmbeddingRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("content")] GeminiContent Content);

internal record GeminiEmbeddingResponse(
    [property: JsonPropertyName("embedding")] GeminiEmbeddingValues Embedding);

internal record GeminiEmbeddingValues(
    [property: JsonPropertyName("values")] float[] Values);
