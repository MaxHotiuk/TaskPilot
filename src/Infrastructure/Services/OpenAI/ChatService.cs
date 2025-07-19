using Application.Abstractions.Messaging;
using Application.Common.Dtos.Chat;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Infrastructure.Services.OpenAI;

public class ChatService : IChatService
{
    private readonly IKernelMemory _memory;
    private readonly ILogger<ChatService> _logger;

    private const string PromptTemplate = @"\nYou are a helpful assistant for a task management application. \nAnswer the user's question based on the following FAQ information.\nIf the information is not available in the FAQ, politely say so and suggest contacting support.\n\nFAQ Information:\n{0}\n\nUser Question: {1}\n\nResponse:";
    private const string ErrorResponse = "I apologize, but I'm having trouble processing your request right now. Please try again later or contact support.";

    public ChatService(IKernelMemory memory, ILogger<ChatService> logger)
    {
        _memory = memory;
        _logger = logger;
    }

    public async Task<ChatResponse> GetResponseAsync(ChatRequest request)
    {
        try
        {
            var searchResult = await _memory.SearchAsync(
                query: request.Message ?? string.Empty,
                filter: MemoryFilters.ByTag("type", "faq"),
                limit: 3
            );

            var context = string.Join("\n\n", searchResult?.Results?.Select(r => r.Partitions.FirstOrDefault()?.Text) ?? Enumerable.Empty<string>());

            var prompt = string.Format(PromptTemplate, context, request.Message);

            var response = await _memory.AskAsync(prompt);

            return new ChatResponse
            {
                Response = response.Result,
                Sources = searchResult?.Results?.Select(r => r.DocumentId ?? string.Empty).ToList() ?? new List<string>(),
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
}
