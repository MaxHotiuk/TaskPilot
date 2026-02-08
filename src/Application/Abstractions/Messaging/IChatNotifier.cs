using Domain.Dtos.Chats;

namespace Application.Abstractions.Messaging;

public interface IChatNotifier
{
    Task NotifyChatMessageAsync(Guid chatId, ChatMessageDto message);
    Task NotifyChatCreatedAsync(IEnumerable<Guid> userIds, ChatDto chat);
    Task NotifyChatUpdatedAsync(IEnumerable<Guid> userIds, ChatDto chat);
}
