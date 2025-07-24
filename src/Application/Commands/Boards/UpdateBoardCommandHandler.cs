using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Boards;

public class UpdateBoardCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBoardCommand>
{

    private readonly IBoardNotifier _boardNotifier;

    public UpdateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.Id, cancellationToken);
            if (board is null)
            {
                throw new NotFoundException($"Board with ID {request.Id} was not found");
            }

            var oldBoard = new Board
            {
                Id = board.Id,
                Name = board.Name,
                Description = board.Description,
                OwnerId = board.OwnerId,
                CreatedAt = board.CreatedAt,
                UpdatedAt = board.UpdatedAt
            };

            board.Name = request.Name;
            board.Description = request.Description;
            board.UpdatedAt = DateTime.UtcNow;

            var backlogEntry = Application.Common.Helpers.BacklogEntryHelper.CreateBacklogForBoardChange(oldBoard, board);
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            unitOfWork.Boards.Update(board);


            await _boardNotifier.NotifyBoardUpdatedAsync(board.Id.ToString(), new { action = "updated", boardId = board.Id });
        }, cancellationToken);
    }
}
