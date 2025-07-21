using System;
using MediatR;

namespace Application.Commands.Notifications;

public record DeleteNotificationCommand(Guid NotificationId) : IRequest;
