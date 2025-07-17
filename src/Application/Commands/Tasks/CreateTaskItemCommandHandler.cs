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

    public CreateTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
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
            
            await _boardNotifier.NotifyTaskUpdatedAsync(taskItem.Id.ToString(), new { action = "created", taskId = taskItem.Id });
            return taskItem.Id;
        }, cancellationToken);
    }
}
