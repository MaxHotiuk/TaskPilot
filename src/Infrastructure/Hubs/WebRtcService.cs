using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Hubs;

public class WebRtcHub : Hub
{
    private readonly ILogger<WebRtcHub> _logger;

    public WebRtcHub(ILogger<WebRtcHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinBoardGroup(string boardId)
    {
        _logger.LogInformation("Joining board group: {BoardId}", boardId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"board-{boardId}");
    }

    public async Task LeaveBoardGroup(string boardId)
    {
        _logger.LogInformation("Leaving board group: {BoardId}", boardId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board-{boardId}");
    }

    public async Task Send(string message, string boardId)
    {
        _logger.LogInformation("Sending message to board {BoardId}: {Message}", boardId, message);
        await Clients.OthersInGroup($"board-{boardId}").SendAsync("Receive", message);
    }
}