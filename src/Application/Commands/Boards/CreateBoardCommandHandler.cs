using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Boards;

public class CreateBoardCommandHandler : BaseCommandHandler, IRequestHandler<CreateBoardCommand, Guid>
{

    private readonly IBoardNotifier _boardNotifier;

    public CreateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = new Board
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Boards.AddAsync(board, cancellationToken);

            var backlogEntry = new Domain.Entities.Backlog
            {
                BoardId = board.Id,
                Description = $"Board '{board.Name}' was created."
            };
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            await _boardNotifier.NotifyBoardUpdatedAsync(board.Id.ToString(), new { action = "created", boardId = board.Id });
            return board.Id;
        }, cancellationToken);
    }
}
