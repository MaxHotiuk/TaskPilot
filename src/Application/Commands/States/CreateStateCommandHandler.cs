using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.States;

public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, int>
{
    private readonly IStateRepository _stateRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStateCommandHandler(
        IStateRepository stateRepository,
        IBoardRepository boardRepository,
        IUnitOfWork unitOfWork)
    {
        _stateRepository = stateRepository;
        _boardRepository = boardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            throw new ValidationException($"Board with ID {request.BoardId} does not exist");
        }

        var existingState = await _stateRepository.GetStateByBoardAndNameAsync(request.BoardId, request.Name, cancellationToken);
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

        await _stateRepository.AddAsync(state, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return state.Id;
    }
}
