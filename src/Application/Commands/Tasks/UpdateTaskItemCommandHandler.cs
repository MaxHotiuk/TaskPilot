using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Tasks;

public class UpdateTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<UpdateTaskItemCommand>
{

    private readonly IBoardNotifier _boardNotifier;

    public UpdateTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
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

            taskItem.Title = request.Title;
            taskItem.Description = request.Description;
            taskItem.StateId = request.StateId;
            taskItem.AssigneeId = request.AssigneeId;
            taskItem.DueDate = request.DueDate;
            taskItem.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Tasks.Update(taskItem);

            await _boardNotifier.NotifyTaskUpdatedAsync(taskItem.Id.ToString(), new { action = "updated", taskId = taskItem.Id });
        }, cancellationToken);
    }
}
