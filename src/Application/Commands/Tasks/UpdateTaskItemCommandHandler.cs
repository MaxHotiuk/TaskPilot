using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.Tasks;

public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IStateRepository _stateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskItemCommandHandler(
        ITaskItemRepository taskItemRepository,
        IStateRepository stateRepository,
        IUnitOfWork unitOfWork)
    {
        _taskItemRepository = taskItemRepository;
        _stateRepository = stateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (taskItem is null)
        {
            throw new NotFoundException($"Task with ID {request.Id} was not found");
        }

        // Validate state belongs to the board
        if (!await _stateRepository.IsValidStateForBoardAsync(request.StateId, taskItem.BoardId, cancellationToken))
        {
            throw new ValidationException($"State with ID {request.StateId} is not valid for board {taskItem.BoardId}");
        }

        taskItem.Title = request.Title;
        taskItem.Description = request.Description;
        taskItem.StateId = request.StateId;
        taskItem.AssigneeId = request.AssigneeId;
        taskItem.DueDate = request.DueDate;
        taskItem.UpdatedAt = DateTime.UtcNow;

        _taskItemRepository.Update(taskItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
