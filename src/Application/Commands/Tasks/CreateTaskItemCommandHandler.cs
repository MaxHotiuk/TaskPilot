using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IStateRepository _stateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskItemCommandHandler(
        ITaskItemRepository taskItemRepository,
        IBoardRepository boardRepository,
        IStateRepository stateRepository,
        IUnitOfWork unitOfWork)
    {
        _taskItemRepository = taskItemRepository;
        _boardRepository = boardRepository;
        _stateRepository = stateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        // Validate board exists
        var board = await _boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            throw new ValidationException($"Board with ID {request.BoardId} does not exist");
        }

        // Validate state belongs to the board
        if (!await _stateRepository.IsValidStateForBoardAsync(request.StateId, request.BoardId, cancellationToken))
        {
            throw new ValidationException($"State with ID {request.StateId} is not valid for board {request.BoardId}");
        }

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            BoardId = request.BoardId,
            Title = request.Title,
            Description = request.Description,
            StateId = request.StateId,
            AssigneeId = request.AssigneeId,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _taskItemRepository.AddAsync(taskItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return taskItem.Id;
    }
}
