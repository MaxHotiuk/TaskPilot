using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Notifications;

public class CreateNotificationCommandHandler : BaseCommandHandler, IRequestHandler<CreateNotificationCommand>
{
    private readonly IEmailService _emailService;
    private readonly INotificationNotifier _notificationNotifier;

    public CreateNotificationCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IEmailService emailService,
        INotificationNotifier notificationNotifier)
        : base(unitOfWorkFactory)
    {
        _emailService = emailService;
        _notificationNotifier = notificationNotifier;
    }

    public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        Notification? notification = null;
        string? userEmail = null;

        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            notification = unitOfWork.Notifications.BuildNotification(
                request.UserId,
                request.Type,
                request.BoardId,
                request.TaskId,
                request.CustomText,
                request.BoardName,
                request.TaskName,
                request.UserComment
            );
            await unitOfWork.Notifications.AddAsync(notification, cancellationToken);

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is not null)
            {
                userEmail = user.Email;
            }
        }, cancellationToken);

        if (notification is null)
        {
            return;
        }

        await _notificationNotifier.NotifyUserAsync(request.UserId, notification);

        if (userEmail is not null)
        {
            var subject = request.Type switch
            {
                NotificationType.AddedToBoard    => $"You've been added to board: {request.BoardName}",
                NotificationType.AssignedToTask  => $"You've been assigned to task: {request.TaskName}",
                NotificationType.CommentedOnTask => $"New comment on your task: {request.TaskName}",
                _                                => "TaskPilot Notification"
            };

            await _emailService.SendSystemEmailAsync(userEmail, subject, notification.Text, cancellationToken);
        }
    }
}
