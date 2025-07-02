using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class TaskItemRepository : Repository<TaskItem, Guid>, ITaskItemRepository
{
    public TaskItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.State)
            .Include(t => t.Assignee)
            .Where(t => t.BoardId == boardId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByStateIdAsync(int stateId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Assignee)
            .Where(t => t.StateId == stateId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.State)
            .Include(t => t.Board)
            .Where(t => t.AssigneeId == assigneeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetTaskWithCommentsAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Comments.OrderBy(c => c.CreatedAt))
                .ThenInclude(c => c.Author)
            .Include(t => t.State)
            .Include(t => t.Assignee)
            .Include(t => t.Board)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet
            .Include(t => t.Assignee)
            .Include(t => t.Board)
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now)
            .ToListAsync(cancellationToken);
    }
}
