using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Domain.Dtos.Chat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using System.Net.Http.Json;

namespace Infrastructure.Services.OpenAI;

public class ChatService : IChatService
{
    private readonly IKernelMemory _memory;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<ChatService> _logger;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly string _faqJsonPath;

    private const string GeminiEndpoint =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    private const string PromptTemplate =
        "You are a helpful assistant for a task management application called TaskPilot.\n" +
        "You are currently assisting a user from the organisation: {5}.\n" +
        "Current User Context: {6}\n" +
        "Answer the user's question based on the following context.\n" +
        "All the answers should be in Ukrainian.\n" +
        "If the information is not available, politely say so and suggest contacting support.\n\n" +
        "FAQ Information:\n{0}\n\n" +
        "Relevant Task Context:\n{1}\n\n" +
        "Relevant Board Context:\n{2}\n\n" +
        "Relevant Meeting Context:\n{3}\n\n" +
        "User Question: {4}\n\n" +
        "Response:";

    private const string ErrorResponse =
        "Перепрошую, але зараз у мене виникли проблеми з обробкою вашого запиту. Будь ласка, спробуйте пізніше або зверніться до служби підтримки.";

    public ChatService(
        IKernelMemory memory,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ChatService> logger,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        _memory = memory;
        _httpClient = httpClientFactory.CreateClient("Gemini");
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
        _logger = logger;
        _unitOfWorkFactory = unitOfWorkFactory;
        var faqFileName = configuration["FAQ:FileName"] ?? "faqData.json";
        _faqJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, faqFileName);
    }

    public async Task<ChatResponse> GetResponseAsync(ChatRequest request)
    {
        try
        {
            var userMessage = request.Message ?? string.Empty;

            var faqContext = await LoadFaqContextAsync();

            var taskContext = string.Empty;
            var boardContext = string.Empty;
            var meetingContext = string.Empty;
            var organizationName = "Unknown Organisation";
            var userContext = "No user information available";
            SearchResult? taskSearchResult = null;
            SearchResult? boardSearchResult = null;
            SearchResult? meetingSearchResult = null;

            if (request.OrganizationId.HasValue)
            {
                var orgId = request.OrganizationId.Value.ToString();

                using var unitOfWork = await _unitOfWorkFactory.CreateAsync();
                var org = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId.Value);
                if (org is not null)
                    organizationName = org.Name;

                if (request.UserId.HasValue)
                {
                    var user = await unitOfWork.Users.GetByIdAsync(request.UserId.Value);
                    if (user is not null)
                    {
                        userContext = $"User ID: {user.Id}, Username: {user.Username}, Email: {user.Email}";
                    }
                }

                var taskFilter = new MemoryFilter()
                    .ByTag("type", "task")
                    .ByTag("OrganizationId", orgId);

                if (request.UserId.HasValue)
                {
                    taskFilter = taskFilter.ByTag("UserId", request.UserId.Value.ToString());
                }

                taskSearchResult = await _memory.SearchAsync(
                    query: userMessage,
                    filter: taskFilter,
                    limit: 5
                );

                taskContext = string.Join("\n\n",
                    taskSearchResult?.Results?.Select(r => r.Partitions.FirstOrDefault()?.Text)
                    ?? Enumerable.Empty<string>());

                var boardFilter = new MemoryFilter()
                    .ByTag("type", "board")
                    .ByTag("OrganizationId", orgId);

                boardSearchResult = await _memory.SearchAsync(
                    query: userMessage,
                    filter: boardFilter,
                    limit: 5
                );

                boardContext = string.Join("\n\n",
                    boardSearchResult?.Results?.Select(r => r.Partitions.FirstOrDefault()?.Text)
                    ?? Enumerable.Empty<string>());

                var meetingFilter = new MemoryFilter()
                    .ByTag("type", "meeting")
                    .ByTag("OrganizationId", orgId);

                meetingSearchResult = await _memory.SearchAsync(
                    query: userMessage,
                    filter: meetingFilter,
                    limit: 5
                );

                meetingContext = string.Join("\n\n",
                    meetingSearchResult?.Results?.Select(r => r.Partitions.FirstOrDefault()?.Text)
                    ?? Enumerable.Empty<string>());
            }

            var prompt = string.Format(PromptTemplate, faqContext, taskContext, boardContext, meetingContext, userMessage, organizationName, userContext);

            var generatedText = await CallGeminiAsync(prompt);

            var sources = new List<string>();
            sources.AddRange(taskSearchResult?.Results?.Select(r => r.DocumentId ?? string.Empty) ?? Enumerable.Empty<string>());
            sources.AddRange(boardSearchResult?.Results?.Select(r => r.DocumentId ?? string.Empty) ?? Enumerable.Empty<string>());
            sources.AddRange(meetingSearchResult?.Results?.Select(r => r.DocumentId ?? string.Empty) ?? Enumerable.Empty<string>());

            return new ChatResponse
            {
                Response = generatedText,
                Sources = sources,
                SessionId = request.SessionId ?? throw new ArgumentNullException(nameof(request.SessionId))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating chat response");
            return new ChatResponse
            {
                Response = ErrorResponse,
                Sources = new List<string>(),
                SessionId = request.SessionId ?? throw new ArgumentNullException(nameof(request.SessionId))
            };
        }
    }

    private async Task<string> LoadFaqContextAsync()
    {
        if (!File.Exists(_faqJsonPath))
            return string.Empty;

        try
        {
            var json = await File.ReadAllTextAsync(_faqJsonPath);
            var items = System.Text.Json.JsonSerializer.Deserialize<List<Domain.Dtos.Chat.FAQItem>>(json);
            if (items is null or { Count: 0 })
                return string.Empty;

            return string.Join("\n", items.Select(f => $"Q: {f.Question}\nA: {f.Answer}"));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load FAQ context from {Path}", _faqJsonPath);
            return string.Empty;
        }
    }

    private async Task<string> CallGeminiAsync(string prompt)
    {
        var requestBody = new GeminiRequest(
            Contents: [new GeminiContent(Parts: [new GeminiPart(Text: prompt)])]
        );

        var url = $"{GeminiEndpoint}?key={_apiKey}";

        var httpResponse = await _httpClient.PostAsJsonAsync(url, requestBody);
        httpResponse.EnsureSuccessStatusCode();

        var geminiResponse = await httpResponse.Content.ReadFromJsonAsync<GeminiResponse>()
            ?? throw new InvalidOperationException("Received a null response from the Gemini API.");

        return geminiResponse.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
            ?? ErrorResponse;
    }
}

