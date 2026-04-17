using Application.Abstractions.Messaging;
using Hangfire;

namespace Infrastructure.BackgroundJobs;

public class AiSyncEnqueuer : IAiSyncEnqueuer
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public AiSyncEnqueuer(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void EnqueueSync(Guid taskId)
    {
        _backgroundJobClient.Enqueue<IAiContextSyncService>(s => s.SyncTaskItemAsync(taskId));
    }

    public void EnqueueDelete(Guid taskId)
    {
        _backgroundJobClient.Enqueue<IAiContextSyncService>(s => s.DeleteTaskItemContextAsync(taskId));
    }

    public void EnqueueSyncBoard(Guid boardId)
    {
        _backgroundJobClient.Enqueue<IAiContextSyncService>(s => s.SyncBoardAsync(boardId));
    }

    public void EnqueueDeleteBoard(Guid boardId)
    {
        _backgroundJobClient.Enqueue<IAiContextSyncService>(s => s.DeleteBoardContextAsync(boardId));
    }

    public void EnqueueSyncMeeting(Guid meetingId)
    {
        _backgroundJobClient.Enqueue<IAiContextSyncService>(s => s.SyncMeetingAsync(meetingId));
    }

    public void EnqueueDeleteMeeting(Guid meetingId)
    {
        _backgroundJobClient.Enqueue<IAiContextSyncService>(s => s.DeleteMeetingContextAsync(meetingId));
    }
}
