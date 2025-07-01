using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ITaskItemRepository : IRepository<TaskItem, Guid>
{
    Task<IEnumerable<TaskItem>> GetTasksByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetTasksByStateIdAsync(int stateId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetTasksByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetTaskWithCommentsAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
}
