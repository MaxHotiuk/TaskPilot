using Domain.Dtos.Notifications;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class NotificationMappingExtensions
{
    public static NotificationDto ToDto(this Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Text = notification.Text,
            Type = notification.Type,
            BoardId = notification.BoardId,
            TaskId = notification.TaskId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
