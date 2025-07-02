using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTaskItemByIdQueryHandler : IRequestHandler<GetTaskItemByIdQuery, TaskItemDto?>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public GetTaskItemByIdQueryHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<TaskItemDto?> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (taskItem is null)
        {
            return null;
        }

        return new TaskItemDto
        {
            Id = taskItem.Id,
            BoardId = taskItem.BoardId,
            Title = taskItem.Title,
            Description = taskItem.Description,
            StateId = taskItem.StateId,
            AssigneeId = taskItem.AssigneeId,
            DueDate = taskItem.DueDate,
            CreatedAt = taskItem.CreatedAt,
            UpdatedAt = taskItem.UpdatedAt
        };
    }
}
