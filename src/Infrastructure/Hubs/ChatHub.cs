using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChatGroup(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
    }

    public async Task LeaveChatGroup(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
    }

    public async Task JoinUserGroup(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-user-{userId}");
        }
    }

    public async Task LeaveUserGroup(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-user-{userId}");
        }
    }

    public Task StartTyping(string chatId, string userId)
    {
        return Clients.Group($"chat-{chatId}").SendAsync("UserTyping", new { chatId, userId });
    }

    public Task StopTyping(string chatId, string userId)
    {
        return Clients.Group($"chat-{chatId}").SendAsync("UserStoppedTyping", new { chatId, userId });
    }
}
