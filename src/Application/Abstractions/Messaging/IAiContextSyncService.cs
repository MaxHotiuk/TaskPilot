namespace Application.Abstractions.Messaging;

public interface IAiContextSyncService
{
    Task SyncTaskItemAsync(Guid taskId);
    Task DeleteTaskItemContextAsync(Guid taskId);
    Task SyncBoardAsync(Guid boardId);
    Task DeleteBoardContextAsync(Guid boardId);
    Task SyncMeetingAsync(Guid meetingId);
    Task DeleteMeetingContextAsync(Guid meetingId);
}
