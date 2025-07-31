using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions.Persistence;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Notification BuildNotification(
        Guid userId,
        NotificationType type,
        Guid? boardId = null,
        Guid? taskId = null,
        string? customText = null,
        string? boardName = null,
        string? taskName = null,
        string? userComment = null);
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetNotificationsRangeByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
}
