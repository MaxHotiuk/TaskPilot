using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Boards;

public class DeleteBoardCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBoardCommand>
{
    public DeleteBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
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

            unitOfWork.Boards.Remove(board);
        }, cancellationToken);
    }
}
