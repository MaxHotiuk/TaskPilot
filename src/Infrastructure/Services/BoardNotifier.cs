using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.Hubs;

namespace Infrastructure.Services;

public class BoardNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub> _hubContext;

    public BoardNotifier(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyBoardUpdatedAsync(string boardId, object payload)
    {
        return _hubContext.Clients.Group($"board-{boardId}").SendAsync("BoardUpdated", payload);
    }

    public Task NotifyTaskUpdatedAsync(string taskId, object payload)
    {
        return _hubContext.Clients.Group($"task-{taskId}").SendAsync("TaskUpdated", payload);
    }

    public Task NotifyUserAddedToBoardAsync(string userId, string boardId, string addedBy, string boardName)
    {
        return _hubContext.Clients.User(userId).SendAsync("UserAddedToBoard", new { boardId, boardName, addedBy });
    }
}
