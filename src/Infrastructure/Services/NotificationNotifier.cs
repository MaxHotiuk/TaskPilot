using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Infrastructure.Hubs;
using Application.Abstractions.Messaging;

namespace Infrastructure.Services;

public class NotificationNotifier : INotificationNotifier
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationNotifier(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUserAsync(Guid userId, Notification notification)
    {
        await _hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
        {
            notification.Id,
            notification.Text,
            notification.Type,
            notification.BoardId,
            notification.TaskId,
            notification.IsRead,
            notification.CreatedAt
        });
    }
}
