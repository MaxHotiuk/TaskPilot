using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTaskItemsByBoardIdQueryHandler : IRequestHandler<GetTaskItemsByBoardIdQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public GetTaskItemsByBoardIdQueryHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetTaskItemsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await _taskItemRepository.GetTasksByBoardIdAsync(request.BoardId, cancellationToken);

        return taskItems.Select(taskItem => new TaskItemDto
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
        });
    }
}
