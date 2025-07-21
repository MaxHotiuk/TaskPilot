using MediatR;

namespace Application.Commands.Notifications;

public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest;
