using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Boards;

public class DeleteBoardCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBoardCommand>
{

    private readonly IBoardNotifier _boardNotifier;
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public DeleteBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.Id, cancellationToken);
            
            if (board is null)
            {
                throw new NotFoundException($"Board with ID {request.Id} was not found");
            }

            var boardChat = await unitOfWork.Chats.GetBoardChatAsync(board.Id, cancellationToken);
            if (boardChat is not null)
            {
                unitOfWork.Chats.Remove(boardChat);
            }

            var notifications = await unitOfWork.Notifications
                .FindAsync(notification => notification.BoardId == board.Id, cancellationToken);
            foreach (var notification in notifications)
            {
                unitOfWork.Notifications.Remove(notification);
            }

            unitOfWork.Boards.Remove(board);

            await _boardNotifier.NotifyBoardUpdatedAsync(board.Id.ToString(), new { action = "deleted", boardId = board.Id });
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueDeleteBoard(request.Id);
    }
}
