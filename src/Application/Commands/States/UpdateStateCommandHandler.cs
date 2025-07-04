using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.States;

public class UpdateStateCommandHandler : BaseCommandHandler, IRequestHandler<UpdateStateCommand>
{
    public UpdateStateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var state = await unitOfWork.States.GetByIdAsync(request.Id, cancellationToken);
            
            if (state is null)
            {
                throw new NotFoundException($"State with ID {request.Id} was not found");
            }

            var existingState = await unitOfWork.States.GetStateByBoardAndNameAsync(state.BoardId, request.Name, cancellationToken);
            if (existingState is not null && existingState.Id != request.Id)
            {
                throw new ValidationException($"State with name '{request.Name}' already exists in this board");
            }

            state.Name = request.Name;
            state.Order = request.Order;
            state.UpdatedAt = DateTime.UtcNow;

            unitOfWork.States.Update(state);
        }, cancellationToken);
    }
}
