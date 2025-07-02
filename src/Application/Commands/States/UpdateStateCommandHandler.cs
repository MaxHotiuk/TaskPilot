using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.States;

public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand>
{
    private readonly IStateRepository _stateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStateCommandHandler(IStateRepository stateRepository, IUnitOfWork unitOfWork)
    {
        _stateRepository = stateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _stateRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (state is null)
        {
            throw new NotFoundException($"State with ID {request.Id} was not found");
        }

        // Check if state with same name already exists in the board (excluding current state)
        var existingState = await _stateRepository.GetStateByBoardAndNameAsync(state.BoardId, request.Name, cancellationToken);
        if (existingState is not null && existingState.Id != request.Id)
        {
            throw new ValidationException($"State with name '{request.Name}' already exists in this board");
        }

        state.Name = request.Name;
        state.Order = request.Order;
        state.UpdatedAt = DateTime.UtcNow;

        _stateRepository.Update(state);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
