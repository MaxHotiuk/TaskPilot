using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Tasks;

public class DeleteTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<DeleteTaskItemCommand>
{

    private readonly IBoardNotifier _boardNotifier;

    public DeleteTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var taskItem = await unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            
            if (taskItem is null)
            {
                throw new NotFoundException($"Task with ID {request.Id} was not found");
            }

            unitOfWork.Tasks.Remove(taskItem);

            await unitOfWork.Boards.TouchBoardAsync(taskItem.BoardId, cancellationToken);

            await _boardNotifier.NotifyBoardUpdatedAsync(taskItem.BoardId.ToString(), new { action = "deleted", boardId = taskItem.BoardId });
        }, cancellationToken);
    }
}
