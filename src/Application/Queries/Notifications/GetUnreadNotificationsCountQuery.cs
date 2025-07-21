using MediatR;

namespace Application.Queries.Notifications;

public record GetUnreadNotificationsCountQuery(Guid UserId) : IRequest<int>;
