using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;
using Domain.Dtos.Tasks;

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
            .OrderBy(t => t.DueDate)
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

    public async Task<IEnumerable<TaskCalendarItemDto>> GetTasksForCalendarAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.AssigneeId == userId && t.DueDate.HasValue && t.DueDate.Value >= startDate && t.DueDate.Value <= endDate)
            .Select(t => new TaskCalendarItemDto
            {
                Id = t.Id,
                BoardId = t.BoardId,
                Title = t.Title,
                DueDate = t.DueDate
            })
            .ToListAsync(cancellationToken);
    }
}
