using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks;

public class RestoreTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<RestoreTaskItemCommand>
{

    private readonly IBoardNotifier _boardNotifier;
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public RestoreTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task Handle(RestoreTaskItemCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var task = await unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
            if (task is null)
            {
                throw new ValidationException($"Task with ID {request.TaskId} does not exist");
            }

            await unitOfWork.Tasks.RestoreTaskAsync(request.TaskId, cancellationToken);

            await unitOfWork.Boards.TouchBoardAsync(task.BoardId, cancellationToken);

            await _boardNotifier.NotifyBoardUpdatedAsync(task.BoardId.ToString(), new { action = "restored", boardId = task.BoardId });
            return task.Id;
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueSync(request.TaskId);
    }
}
