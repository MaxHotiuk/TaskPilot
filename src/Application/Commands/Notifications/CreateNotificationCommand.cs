using System;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Notifications;

public record CreateNotificationCommand(
    Guid UserId,
    NotificationType Type,
    Guid? BoardId = null,
    Guid? TaskId = null,
    string? BoardName = null,
    string? TaskName = null,
    string? UserComment = null,
    string? CustomText = null
) : IRequest;
