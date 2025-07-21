using System;
using MediatR;

namespace Application.Commands.Notifications;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest;
