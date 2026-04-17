using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Persistence;

namespace Infrastructure.Services.OpenAI;

public class AiContextSyncService : IAiContextSyncService
{
    private readonly IKernelMemory _memory;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AiContextSyncService> _logger;

    public AiContextSyncService(
        IKernelMemory memory,
        ApplicationDbContext dbContext,
        ILogger<AiContextSyncService> logger)
    {
        _memory = memory;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SyncTaskItemAsync(Guid taskId)
    {
        var task = await _dbContext.Tasks
            .Include(t => t.Board)
            .Include(t => t.Assignee)
            .Include(t => t.State)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for AI context sync", taskId);
            return;
        }

        var assigneeName = task.Assignee?.Username ?? "Unassigned";
        var stateName = task.State?.Name ?? "Unknown";
        var description = string.IsNullOrWhiteSpace(task.Description) ? "N/A" : task.Description;
        var dueDate = task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd") : "None";

        var text = $"Task:{task.Title}|Status:{stateName}|Assignee:{assigneeName}|Priority:{task.Priority}|Due:{dueDate}|Desc:{description}";

        await _memory.ImportTextAsync(
            text,
            documentId: $"task-{taskId}",
            tags: new TagCollection
            {
                { "type", "task" },
                { "OrganizationId", task.Board.OrganizationId.ToString() }
            }
        );

        _logger.LogInformation("Synced AI context for task {TaskId}", taskId);
    }

    public async Task DeleteTaskItemContextAsync(Guid taskId)
    {
        try
        {
            await _memory.DeleteDocumentAsync($"task-{taskId}");
            _logger.LogInformation("Deleted AI context for task {TaskId}", taskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI context for task {TaskId}", taskId);
        }
    }

    public async Task SyncBoardAsync(Guid boardId)
    {
        var board = await _dbContext.Boards
            .FirstOrDefaultAsync(b => b.Id == boardId);

        if (board is null)
        {
            _logger.LogWarning("Board {BoardId} not found for AI context sync", boardId);
            return;
        }

        var status = board.IsArchived ? "Archived" : "Active";
        var description = string.IsNullOrWhiteSpace(board.Description) ? "N/A" : board.Description;

        var text = $"Board:{board.Name}|Status:{status}|Desc:{description}";

        await _memory.ImportTextAsync(
            text,
            documentId: $"board-{boardId}",
            tags: new TagCollection
            {
                { "type", "board" },
                { "OrganizationId", board.OrganizationId.ToString() }
            }
        );

        _logger.LogInformation("Synced AI context for board {BoardId}", boardId);
    }

    public async Task DeleteBoardContextAsync(Guid boardId)
    {
        try
        {
            await _memory.DeleteDocumentAsync($"board-{boardId}");
            _logger.LogInformation("Deleted AI context for board {BoardId}", boardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI context for board {BoardId}", boardId);
        }
    }

    public async Task SyncMeetingAsync(Guid meetingId)
    {
        var data = await _dbContext.Meetings
            .Where(m => m.Id == meetingId)
            .Select(m => new
            {
                m.Title,
                m.Status,
                m.ScheduledAt,
                m.Duration,
                m.Description,
                m.BoardId,
                OrganizationId = m.Board.OrganizationId
            })
            .FirstOrDefaultAsync();

        if (data is null)
        {
            _logger.LogWarning("Meeting {MeetingId} not found for AI context sync", meetingId);
            return;
        }

        var start = data.ScheduledAt.HasValue ? data.ScheduledAt.Value.ToString("yyyy-MM-ddTHH:mm") : "None";
        var duration = data.Duration.HasValue ? $"{data.Duration}min" : "N/A";
        var description = string.IsNullOrWhiteSpace(data.Description) ? "N/A" : data.Description;

        var text = $"Meeting:{data.Title}|Status:{data.Status}|Start:{start}|Duration:{duration}|Board:{data.BoardId}|Desc:{description}";

        await _memory.ImportTextAsync(
            text,
            documentId: $"meeting-{meetingId}",
            tags: new TagCollection
            {
                { "type", "meeting" },
                { "OrganizationId", data.OrganizationId.ToString() }
            }
        );

        _logger.LogInformation("Synced AI context for meeting {MeetingId}", meetingId);
    }

    public async Task DeleteMeetingContextAsync(Guid meetingId)
    {
        try
        {
            await _memory.DeleteDocumentAsync($"meeting-{meetingId}");
            _logger.LogInformation("Deleted AI context for meeting {MeetingId}", meetingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete AI context for meeting {MeetingId}", meetingId);
        }
    }
}
