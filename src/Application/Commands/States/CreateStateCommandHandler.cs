using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.States;

public class CreateStateCommandHandler : BaseCommandHandler, IRequestHandler<CreateStateCommand, int>
{
    public CreateStateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<int> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            var existingState = await unitOfWork.States.GetStateByBoardAndNameAsync(request.BoardId, request.Name, cancellationToken);
            if (existingState is not null)
            {
                throw new ValidationException($"State with name '{request.Name}' already exists in this board");
            }

            var state = new State
            {
                BoardId = request.BoardId,
                Name = request.Name,
                Order = request.Order,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.States.AddAsync(state, cancellationToken);

            return state.Id;
        }, cancellationToken);
    }
}
