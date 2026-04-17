namespace Application.Abstractions.Messaging;

public interface IAiSyncEnqueuer
{
    void EnqueueSync(Guid taskId);
    void EnqueueDelete(Guid taskId);
    void EnqueueSyncBoard(Guid boardId);
    void EnqueueDeleteBoard(Guid boardId);
    void EnqueueSyncMeeting(Guid meetingId);
    void EnqueueDeleteMeeting(Guid meetingId);
}
