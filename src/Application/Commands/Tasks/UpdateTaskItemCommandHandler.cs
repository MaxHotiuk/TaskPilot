using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Tasks;

public class UpdateTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<UpdateTaskItemCommand>
{

    private readonly IBoardNotifier _boardNotifier;
    private readonly INotificationNotifier _notificationNotifier;

    public UpdateTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, INotificationNotifier notificationNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _notificationNotifier = notificationNotifier;
    }

    public async Task Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var taskItem = await unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            if (taskItem is null)
            {
                throw new NotFoundException($"Task with ID {request.Id} was not found");
            }

            if (!await unitOfWork.States.IsValidStateForBoardAsync(request.StateId, taskItem.BoardId, cancellationToken))
            {
                throw new ValidationException($"State with ID {request.StateId} is not valid for board {taskItem.BoardId}");
            }

            var previousAssigneeId = taskItem.AssigneeId;

            taskItem.Title = request.Title;
            taskItem.Description = request.Description;
            taskItem.StateId = request.StateId;
            taskItem.AssigneeId = request.AssigneeId;
            taskItem.DueDate = request.DueDate;
            taskItem.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Tasks.Update(taskItem);

            if (request.AssigneeId != null && request.AssigneeId != previousAssigneeId)
            {
                var notification = unitOfWork.Notifications.BuildNotification(
                    userId: request.AssigneeId.Value,
                    type: Domain.Enums.NotificationType.AssignedToTask,
                    taskId: taskItem.Id,
                    taskName: taskItem.Title
                );
                await unitOfWork.Notifications.AddAsync(notification, cancellationToken);
                await _notificationNotifier.NotifyUserAsync(request.AssigneeId.Value, notification);
            }

            await _boardNotifier.NotifyBoardUpdatedAsync(taskItem.BoardId.ToString(), new { action = "updated", boardId = taskItem.BoardId });
        }, cancellationToken);
    }
}
