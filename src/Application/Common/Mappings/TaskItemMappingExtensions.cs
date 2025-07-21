using Domain.Dtos.Tasks;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class TaskItemMappingExtensions
{
    public static TaskItemDto ToDto(this TaskItem taskItem)
    {
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

    public static IEnumerable<TaskItemDto> ToDto(this IEnumerable<TaskItem> taskItems)
    {
        return taskItems.Select(ToDto);
    }
}
