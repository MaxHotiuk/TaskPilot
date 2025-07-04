using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Boards;

public class UpdateBoardCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBoardCommand>
{
    public UpdateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
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

            board.Name = request.Name;
            board.Description = request.Description;
            board.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Boards.Update(board);
        }, cancellationToken);
    }
}
