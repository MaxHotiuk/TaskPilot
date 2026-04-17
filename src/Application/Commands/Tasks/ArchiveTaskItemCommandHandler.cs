using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks;

public class ArchiveTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<ArchiveTaskItemCommand>
{

    private readonly IBoardNotifier _boardNotifier;
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public ArchiveTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task Handle(ArchiveTaskItemCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var task = await unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
            if (task is null)
            {
                throw new ValidationException($"Task with ID {request.TaskId} does not exist");
            }

            await unitOfWork.Tasks.ArchiveTaskAsync(request.TaskId, cancellationToken);

            await unitOfWork.Boards.TouchBoardAsync(task.BoardId, cancellationToken);

            await _boardNotifier.NotifyBoardUpdatedAsync(task.BoardId.ToString(), new { action = "archived", boardId = task.BoardId });
            return task.Id;
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueDelete(request.TaskId);
    }
}
