using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Boards;

public class CreateBoardCommandHandler : BaseCommandHandler, IRequestHandler<CreateBoardCommand, Guid>
{
    public CreateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
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
            
            return board.Id;
        }, cancellationToken);
    }
}
