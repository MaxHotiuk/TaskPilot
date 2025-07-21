using Domain.Dtos.Notifications;
using MediatR;

namespace Application.Queries.Notifications;

public record GetNotificationsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<NotificationDto>>;
