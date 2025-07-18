using Application.Common.Dtos.Chat;

namespace Application.Abstractions.Messaging;

public interface IChatService
{
    Task<ChatResponse> GetResponseAsync(ChatRequest request);
}
