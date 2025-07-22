using Domain.Dtos.Notifications;
using MediatR;

namespace Application.Queries.Notifications;

public record GetNotificationsRangeByUserIdQuery(Guid UserId, int Page, int PageSize) : IRequest<IEnumerable<NotificationDto>>;
