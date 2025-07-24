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
            .Where(t => t.BoardId == boardId && !t.IsArchived)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByStateIdAsync(int stateId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Assignee)
            .Where(t => t.StateId == stateId && !t.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.State)
            .Include(t => t.Board)
            .Where(t => t.AssigneeId == assigneeId && !t.IsArchived)
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
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now && !t.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskCalendarItemDto>> GetTasksForCalendarAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.AssigneeId == userId
                && t.DueDate.HasValue
                && t.DueDate.Value >= startDate
                && t.DueDate.Value <= endDate
                && !t.IsArchived)
            .Select(t => new TaskCalendarItemDto
            {
                Id = t.Id,
                BoardId = t.BoardId,
                Title = t.Title,
                DueDate = t.DueDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivedTaskDto>> SearchArchivedRangeByBoardIdAsync(Guid boardId, int page, int pageSize, string searchTerm, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.BoardId == boardId && t.IsArchived && (string.IsNullOrEmpty(searchTerm)
            || t.Title.Contains(searchTerm)
            || (t.Assignee != null && t.Assignee.Username.Contains(searchTerm))))
            .OrderByDescending(t => t.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new ArchivedTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Assignee = t.Assignee != null ? t.Assignee.Username : null,
                DueDate = t.DueDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task ArchiveTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await DbSet.FindAsync(new object[] { taskId }, cancellationToken);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found");
        }

        task.IsArchived = true;
        task.UpdatedAt = DateTime.UtcNow;

        DbSet.Update(task);
    }

    public async Task RestoreTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await DbSet.FindAsync(new object[] { taskId }, cancellationToken);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found");
        }

        task.IsArchived = false;
        task.UpdatedAt = DateTime.UtcNow;

        DbSet.Update(task);
    }
}
