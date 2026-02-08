namespace Application.Abstractions.Meetings;

public interface IDailyRoomService
{
    Task<string> CreateRoomAsync(Guid meetingId, CancellationToken cancellationToken);
}
