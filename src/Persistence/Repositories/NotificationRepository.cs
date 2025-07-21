using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;
using Domain.Enums;

namespace Persistence.Repositories;

public class NotificationRepository : Repository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Notification BuildNotification(
        Guid userId,
        NotificationType type,
        Guid? boardId = null,
        Guid? taskId = null,
        string? customText = null,
        string? boardName = null,
        string? taskName = null,
        string? userComment = null)
    {
        var text = customText ?? type switch
        {
            NotificationType.AddedToBoard => boardName != null
                ? $"You have been added to the board '{boardName}'."
                : "You have been added to a board.",
            NotificationType.AssignedToTask => taskName != null
                ? $"You have been assigned to the task '{taskName}'."
                : "You have been assigned to a task.",
            NotificationType.CommentedOnTask => (taskName != null && userComment != null)
                ? $"A new comment on task '{taskName}': {userComment}"
                : taskName != null
                    ? $"A new comment was added to your task '{taskName}'."
                    : "A new comment was added to your task.",
            _ => "You have a new notification."
        };

        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            BoardId = boardId,
            TaskId = taskId,
            Text = text,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await Context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await Context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
        if (notification != null)
        {
            notification.IsRead = true;
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
