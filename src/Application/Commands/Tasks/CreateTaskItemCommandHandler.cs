using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks;

public class CreateTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<CreateTaskItemCommand, Guid>
{

    private readonly IBoardNotifier _boardNotifier;
    private readonly INotificationNotifier _notificationNotifier;

    public CreateTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, INotificationNotifier notificationNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _notificationNotifier = notificationNotifier;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            if (!await unitOfWork.States.IsValidStateForBoardAsync(request.StateId, request.BoardId, cancellationToken))
            {
                throw new ValidationException($"State with ID {request.StateId} is not valid for board {request.BoardId}");
            }

            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                Title = request.Title,
                Description = request.Description,
                StateId = request.StateId,
                AssigneeId = request.AssigneeId,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Tasks.AddAsync(taskItem, cancellationToken);

            if (request.AssigneeId != null)
            {
                var notification = unitOfWork.Notifications.BuildNotification(
                    userId: request.AssigneeId.Value,
                    type: Domain.Enums.NotificationType.AssignedToTask,
                    taskId: taskItem.Id,
                    boardId: request.BoardId,
                    taskName: taskItem.Title
                );
                await unitOfWork.Notifications.AddAsync(notification, cancellationToken);
                await _notificationNotifier.NotifyUserAsync(request.AssigneeId.Value, notification);
            }

            await _boardNotifier.NotifyBoardUpdatedAsync(request.BoardId.ToString(), new { action = "created", boardId = request.BoardId });
            return taskItem.Id;
        }, cancellationToken);
    }
}
