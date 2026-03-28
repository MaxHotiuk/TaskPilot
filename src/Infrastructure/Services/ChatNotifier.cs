using Application.Abstractions.Messaging;
using Domain.Dtos.Chats;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyChatMessageAsync(Guid chatId, ChatMessageDto message)
    {
        return _hubContext.Clients.Group($"chat-{chatId}").SendAsync("ReceiveChatMessage", message);
    }

    public Task NotifyChatCreatedAsync(IEnumerable<Guid> userIds, ChatDto chat)
    {
        return NotifyUsersAsync(userIds, "ChatCreated", chat);
    }

    public Task NotifyChatUpdatedAsync(IEnumerable<Guid> userIds, ChatDto chat)
    {
        return NotifyUsersAsync(userIds, "ChatUpdated", chat);
    }

    private Task NotifyUsersAsync(IEnumerable<Guid> userIds, string eventName, ChatDto chat)
    {
        var tasks = userIds
            .Distinct()
            .Select(userId => _hubContext.Clients.Group($"chat-user-{userId}").SendAsync(eventName, chat));

        return Task.WhenAll(tasks);
    }
}
