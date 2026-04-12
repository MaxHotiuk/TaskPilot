using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.States;

public class SwapStateOrderCommandHandler : BaseCommandHandler, IRequestHandler<SwapStateOrderCommand>
{
    public SwapStateOrderCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(SwapStateOrderCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            var isValidFirst = await unitOfWork.States.IsValidStateForBoardAsync(request.FirstStateId, request.BoardId, cancellationToken);
            var isValidSecond = await unitOfWork.States.IsValidStateForBoardAsync(request.SecondStateId, request.BoardId, cancellationToken);
            if (!isValidFirst || !isValidSecond)
            {
                throw new ValidationException("One or both states not found in the specified board.");
            }

            await unitOfWork.States.SwapStateOrderAsync(request.FirstStateId, request.SecondStateId, request.BoardId, cancellationToken);

            board.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Boards.Update(board);
        }, cancellationToken);
    }
}
