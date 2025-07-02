using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public class GetOverdueTaskItemsQueryHandler : IRequestHandler<GetOverdueTaskItemsQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public GetOverdueTaskItemsQueryHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetOverdueTaskItemsQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await _taskItemRepository.GetOverdueTasksAsync(cancellationToken);

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
